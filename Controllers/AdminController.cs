using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongNet.Filters;
using QuanLyPhongNet.Models;

namespace QuanLyPhongNet.Controllers
{
    [AdminOnly]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;


        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang Dashboard - thống kê
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            // Tổng số người dùng
            //ViewBag.TotalUsers = _context.Users.Count();
            var TotalUsers = _context.Users.Count();


            // Tổng số máy
            //ViewBag.TotalDevices = _context.Devices.Count();
            var TotalDevices = _context.Devices.Count();

            // Số máy đang sử dụng
            ViewBag.DevicesInUse = _context.UsageRecords.Count(u => u.EndTime == null && u.UserId != 1);
            var DevicesInUse = _context.UsageRecords.Count(u => u.EndTime == null && u.UserId != 1);

            // Doanh thu theo loại máy (tất cả thời gian)
            var statsByDeviceType = _context.UsageRecords
                .Include(u => u.Device)
                .Where(u => u.EndTime != null)
                .ToList()
                .GroupBy(u => u.Device.Type)
                .Select(g => new
                {
                    DeviceName = g.Key,
                    TotalRevenue = g.Sum(x => (decimal)x.UsageTime.Value.TotalHours * x.Device.PricePerHour)
                }).ToList();
            //ViewBag.StatsByDeviceType = statsByDeviceType;

            // Doanh thu hôm nay
            DateTime today = DateTime.Today;
            var todayRevenue = _context.UsageRecords
                .Include(u => u.Device)
                .Where(u => u.EndTime != null && u.EndTime.Value.Date == today)
                .ToList()
                .Sum(x => (decimal)x.UsageTime.Value.TotalHours * x.Device.PricePerHour);
            //ViewBag.TodayRevenue = todayRevenue;

            // Doanh thu tháng này
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var monthRevenue = _context.UsageRecords
                .Include(u => u.Device)
                .Where(u => u.EndTime != null && u.EndTime >= startOfMonth && u.EndTime <= endOfMonth)
                .ToList()
                .Sum(x => (decimal)x.UsageTime.Value.TotalHours * x.Device.PricePerHour);
            //ViewBag.MonthRevenue = monthRevenue;

            // Doanh thu theo từng loại máy trong ngày
            var statsByDeviceTypeToday = _context.UsageRecords
                .Include(u => u.Device)
                .Where(u => u.EndTime != null && u.EndTime.Value.Date == today)
                .ToList()
                .GroupBy(u => u.Device.Type)
                .Select(g => new
                {
                    DeviceName = g.Key,
                    TotalRevenue = g.Sum(x => (decimal)x.UsageTime.Value.TotalHours * x.Device.PricePerHour)
                }).ToList();
            //ViewBag.StatsByDeviceTypeToday = statsByDeviceTypeToday;

            // Doanh thu theo từng loại máy trong tháng
            var statsByDeviceTypeMonth = _context.UsageRecords
                .Include(u => u.Device)
                .Where(u => u.EndTime != null && u.EndTime >= startOfMonth && u.EndTime <= endOfMonth)
                .ToList()
                .GroupBy(u => u.Device.Type)
                .Select(g => new
                {
                    DeviceName = g.Key,
                    TotalRevenue = g.Sum(x => (decimal)x.UsageTime.Value.TotalHours * x.Device.PricePerHour)
                }).ToList();
            //ViewBag.StatsByDeviceTypeMonth = statsByDeviceTypeMonth;

            return Json(new
            {
                TotalUsers,
                TotalDevices,
                DevicesInUse,
                statsByDeviceType,
                todayRevenue,
                monthRevenue,
                statsByDeviceTypeToday,
                statsByDeviceTypeMonth
            });
        }



        // Danh sách máy tính
        public IActionResult Devices()
        {
            var devices = _context.Devices.ToList();
            return View(devices);
        }

        // Thêm máy tính (GET)
        public IActionResult AddDevice()
        {
            return View();
        }

        // Thêm máy tính (POST)
        [HttpPost]
        public IActionResult AddDevice(Device device)
        {
            if (ModelState.IsValid)
            {
                _context.Devices.Add(device);
                _context.SaveChanges();
                return RedirectToAction("Devices");
            }
            return View(device);
        }

        // Sửa máy tính (GET)
        public IActionResult EditDevice(int id)
        {
            var device = _context.Devices.Find(id);
            if (device == null) return NotFound();
            return View(device);
        }

        // Sửa máy tính (POST)
        [HttpPost]
        public IActionResult EditDevice(Device device)
        {
            var existingDevice = _context.Devices.FirstOrDefault(d => d.Id == device.Id);
            if (existingDevice == null)
            {
                return NotFound();
            }

            existingDevice.Type = device.Type;
            existingDevice.PricePerHour = device.PricePerHour;
            //existingDevice.Status = device.Status;

            _context.SaveChanges();
            return RedirectToAction("Devices");
        }

        // Xóa máy tính
        public IActionResult DeleteDevice(int id)
        {
            var device = _context.Devices.Find(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                _context.SaveChanges();
            }
            return RedirectToAction("Devices");
        }

        // Quản lý người dùng
        public ActionResult Users()
        {
            var viewModel = new AllModel
            {
                Users = _context.Users.ToList(),
                UsageRecords = _context.UsageRecords.ToList(),
                Devices = _context.Devices.ToList()
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Users(string searchId)
        {
            // Khởi tạo AllModel
            var model = new AllModel();

            // Truy vấn Users
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchId))
            {
                if (int.TryParse(searchId, out int userId))
                    usersQuery = usersQuery.Where(u => u.Id == userId);
            }

            // Gán dữ liệu vào model
            model.Users = usersQuery.ToList();
            model.UsageRecords = _context.UsageRecords.ToList(); // Lấy toàn bộ UsageRecords
            model.Devices = _context.Devices.ToList();

            return View(model); // Trả về model kiểu AllModel
        }

        // Sửa thông tin người dùng (GET)
        //public IActionResult EditUser(int id)
        //{
        //    var user = _context.Users.Find(id);
        //    if (user == null) return NotFound();
        //    return View(user);
        //}

        //// Sửa thông tin người dùng (POST)
        //[HttpPost]
        //public IActionResult EditUser(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Users.Update(user);
        //        _context.SaveChanges();
        //        return RedirectToAction("Users");
        //    }
        //    return View(user);
        //}


    }
}

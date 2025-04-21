using Microsoft.AspNetCore.Mvc;
using QuanLyPhongNet.Models;

namespace QuanLyPhongNet.Controllers
{
    public class UsageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet, HttpPost]
        public IActionResult Start(string DeviceType)
        {
            if (string.IsNullOrEmpty(DeviceType))
            {
                // Chưa có dữ liệu, chỉ hiển thị trang form
                return View();
            }
            var username = HttpContext.Session.GetString("Username");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(username) || userId == null)
            {
                TempData["Message"] = "Chưa đăng nhập. Vui lòng đăng nhập trước.";
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user.Balance < 1000)
                return Json(new { success = false, message = "Số dư không đủ " });

            // Xác định giá theo loại máy
            var device = _context.Devices
            .Where(d => d.Type == DeviceType && d.Status == "available")
            .FirstOrDefault();

            if (device == null)
                return Content($"Không có máy nào trống với loại: {DeviceType}");



            var availableDevice = _context.Devices
                .FirstOrDefault(d => d.Status == "available" && d.Type == DeviceType);

            if (availableDevice == null)
                return Content("Không có máy nào trống với loại: " + DeviceType);

            availableDevice.Status = "busy";
            _context.Devices.Update(availableDevice);

            var usage = new UsageRecord
            {
                UserId = userId.Value,
                DeviceId = availableDevice.Id,
                StartTime = DateTime.Now,
                // EndTime sẽ cập nhật sau
            };

            _context.UsageRecords.Add(usage);
            _context.SaveChanges();

            return Json(new { success = true, deviceType = DeviceType, deviceId = availableDevice.Id });
        }


    }
}

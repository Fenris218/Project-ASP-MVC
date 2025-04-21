using Microsoft.AspNetCore.Mvc;
using QuanLyPhongNet.Models;

namespace QuanLyPhongNet.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Register() => View();
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            user.Role = "user";  // Mặc định là user
            user.Status = "Online";
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                user.Status = "Online";
                _context.SaveChanges();
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName.ToString());
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Balance", user.Balance.ToString());

                if (user.Role == "admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Start", "Usage");
            }
            else
            {                 // Nếu không tìm thấy user, hiển thị thông báo lỗi
                ViewBag.Error = "Sai thông tin đăng nhập";
                return View();
            }


        }


        public IActionResult Logout()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                string? role = HttpContext.Session.GetString("Role");

                if (userId.HasValue)
                {
                    // Nếu là user, thì cần xử lý kết thúc phiên sử dụng máy
                    if (role == "user")
                    {
                        var usage = _context.UsageRecords
                                            .FirstOrDefault(u => u.UserId == userId.Value && u.EndTime == null);

                        if (usage != null)
                        {
                            usage.EndTime = DateTime.Now;
                            _context.SaveChanges();

                            var device = _context.Devices.FirstOrDefault(d => d.Id == usage.DeviceId);
                            if (device != null)
                            {
                                device.Status = "available";
                                _context.SaveChanges();
                            }
                            var player = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
                            if (player != null)
                            {
                                player.Balance -= usage.TotalCost; // Giả sử phí sử dụng là 1000 VNĐ
                                _context.SaveChanges();
                            }

                        }
                    }

                    // Cập nhật trạng thái người dùng thành "Offline"
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
                    if (user != null)
                    {
                        user.Status = "Offline";
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi kết thúc phiên sử dụng: " + ex.Message);
            }
            finally
            {
                // Xóa toàn bộ session và quay lại trang đăng nhập
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult LogoutOnClose()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                string? role = HttpContext.Session.GetString("Role");

                if (userId.HasValue)
                {
                    if (role == "user")
                    {
                        var usage = _context.UsageRecords
                                            .FirstOrDefault(u => u.UserId == userId.Value && u.EndTime == null);
                        if (usage != null)
                        {
                            usage.EndTime = DateTime.Now;
                            _context.SaveChanges();

                            var device = _context.Devices.FirstOrDefault(d => d.Id == usage.DeviceId);
                            if (device != null)
                            {
                                device.Status = "available";
                                _context.SaveChanges();
                            }

                            var player = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
                            if (player != null)
                            {
                                player.Balance -= usage.TotalCost;
                                _context.SaveChanges();
                            }
                        }
                    }

                    var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
                    if (user != null)
                    {
                        user.Status = "Offline";
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi kết thúc phiên sử dụng: " + ex.Message);
            }
            finally
            {
                HttpContext.Session.Clear();
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult Play()
        {
            ViewData["Title"] = "Game khủng long mất mạng";

            return View();
        }

        [HttpGet]
        public IActionResult NapTien()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NapTien(decimal soTienNap)
        {
            if (soTienNap < 1000 || soTienNap > 10000000)
            {
                ModelState.AddModelError("NapTienLoi", "Số tiền phải từ 1.000 đến 10.000.000 VNĐ");
                ViewBag.soTienNap = soTienNap;
                return View("Start"); // <- dùng lại View Start.cshtml
            }

            // Lấy thông tin user từ session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null)
                return RedirectToAction("Login");

            user.Balance += soTienNap;
            _context.SaveChanges();

            //TempData["Success"] = $"Đã nạp {soTienNap:N0} VNĐ";
            return Json(new { success = true, tiennap = soTienNap });
        }
        [HttpGet]
        public IActionResult GetBalance()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false });

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Json(new { success = false });

            // Cập nhật session
            HttpContext.Session.SetString("Balance", user.Balance.ToString());

            return Json(new { success = true, balance = user.Balance });
        }
        [HttpGet]
        public IActionResult GetFullName()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false });

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Json(new { success = false });

            // Cập nhật session
            HttpContext.Session.SetString("FullName", user.FullName.ToString());

            return Json(new { success = true, fullname = user.FullName });
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng" });

            return Json(new
            {
                success = true,
                fullName = user.FullName,
                phone = user.Phone,
                password = user.Password
            });
        }
        [HttpPost]
        public IActionResult UpdateProfile(string FullName, string Phone, string Password)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng" });

            user.FullName = FullName;
            user.Phone = Phone;
            user.Password = Password;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Json(new { success = true, message = "Cập nhật thành công!" });
        }



        public IActionResult AccessDenied() => View();
    }
}

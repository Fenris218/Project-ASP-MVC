using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongNet.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string Phone { get; set; }
        public string Status { get; set; } // "Online" hoặc "Offline"
        public decimal Balance { get; set; }
        public string Role { get; set; }  // "admin" hoặc "user"
    }

}

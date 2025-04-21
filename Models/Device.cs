namespace QuanLyPhongNet.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Type { get; set; } //"Standard" ,"Vip"
        public string Status { get; set; }  // "Available", "InUse"
        public decimal PricePerHour { get; set; }

    }

}

namespace QuanLyPhongNet.Models
{
    public class UsageRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DeviceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public User User { get; set; }
        public Device Device { get; set; }

        public TimeSpan? UsageTime
        {
            get
            {
                if (EndTime.HasValue)
                    return EndTime.Value - StartTime;
                return null;
            }
            set { }
        }
        public decimal TotalCost
        {
            get
            {
                if (UsageTime.HasValue && Device != null)
                {
                    return (decimal)UsageTime.Value.TotalHours * Device.PricePerHour;
                }
                return 0;
            }
            set { }
        }
    }

}

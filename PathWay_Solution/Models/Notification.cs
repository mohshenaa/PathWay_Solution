using PathWay_Solution.IdentityModels;

namespace PathWay_Solution.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public Guid UserId { get; set; }

        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public AppUser AppUser { get; set; } = null!;
    }

}

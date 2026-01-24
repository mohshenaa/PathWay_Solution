using PathWay_Solution.IdentityModels;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public Guid AppUserId { get; set; }

        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public AppUser AppUser { get; set; } = null!;
    }

}

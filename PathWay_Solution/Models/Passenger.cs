using PathWay_Solution.IdentityModels;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }
        public Guid UserId { get; set; }

        [Required]
        public string Gender { get; set; } = default!;
        public AppUser AppUser { get; set; } = null!;
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<ReviewRating>? Reviews { get; set; }
    }
   
}

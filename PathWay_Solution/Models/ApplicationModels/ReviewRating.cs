using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class ReviewRating
    {
        [Key]
        public int ReviewRatingId { get; set; }
        public int PassengerId { get; set; }
        public int TripId { get; set; }
        public int Rating { get; set; } 
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Passenger Passenger { get; set; } = null!;
        public Trip Trip { get; set; } = null!;
    }

}
using PathWay_Solution.Models.ApplicationModels;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class BookingCreateDto
    {
        [Required]
        public int PassengerId { get; set; }

        [Required]
        public int TripId { get; set; }

        public List<int>? TripSeatId { get; set; }
        public BookingSource BookingSource { get; set; } = BookingSource.Online;

        [Required]
        public decimal TotalAmount { get; set; }
    }

    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int PassengerId { get; set; }
        public int TripId { get; set; }
public List<int>? TripSeatId { get; set; }
        public BookingSource BookingSource { get; set; }
        public BookingStatus Status { get; set; }

        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
    }
}

using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Dto
{

    public class PassengerCreateDto
    {
        public Guid AppUserId { get; set; }
        public string Gender { get; set; } = default!;
    }


    public class PassengerReadDto
    {
        public Guid AppUserId { get; set; }
        public int PassengerId { get; set; }
        public string Gender { get; set; } = default!;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int BookingCount { get; set; }
        public int ReviewCount { get; set; }
    }

    //public class PassengerProfileDto
    //{
    //    public int PassengerId { get; set; }
    //    public Guid AppUserId { get; set; }
    //    public string Gender { get; set; } = null!;

    //    public string UserName { get; set; } = null!;
    //    public string Email { get; set; } = null!;

    //    public int BookingCount { get; set; }
    //    public int ReviewCount { get; set; }

    //    public List<PassengerBookingDto>? Bookings { get; set; } = new();
  //  }

    public class PassengerBookingDto
    {
        public int BookingId { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }

        public string TripStatus { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal? RefundAmount { get; set; }
        public string VehicleNumber { get; set; } = null!;

        public List<string>? Seats { get; set; } = new();

        public string PaymentStatus { get; set; } = null!;
    }
    public class PassengerDashboardDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;

        public int TotalBookings { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }
        public int UpcomingTrips { get; set; }
        public decimal? RefundAmount { get; set; }
        public List<PassengerBookingDto> Upcoming { get; set; } = new();
        public List<PassengerBookingDto> Completed { get; set; } = new();
        public List<PassengerBookingDto> Cancelled { get; set; } = new();
    }
}

namespace PathWay_Solution.Dto
{
    public class CreatePassengerTripDto
    {

        public int TripId { get; set; }
        public string Direction { get; set; } = null!;
        public string VehicleName { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<SeatDto>? Seats { get; set; }
    }

    public class SeatDto
    {
        public int TripSeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public bool IsBooked { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedUntil { get; set; }

    }
}

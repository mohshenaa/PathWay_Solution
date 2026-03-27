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
}

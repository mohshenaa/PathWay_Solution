namespace PathWay_Solution.Dto
{
    public class LocationCreateDto
    {

        public string LocationName { get; set; } = null!;
    }
    public class LocationResponseDto
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
    }
}

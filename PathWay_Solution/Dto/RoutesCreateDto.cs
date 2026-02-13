namespace PathWay_Solution.Dto
{
    public class RoutesCreateDto
    {
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public double DistanceInKm { get; set; }
      
    }
    public class RoutesUpdateDto
    {
        public int RoutesId { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public double DistanceInKm { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

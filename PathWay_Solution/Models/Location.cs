namespace PathWay_Solution.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Routes>? RoutesFrom { get; set; }
        public ICollection<Routes>? RoutesTo { get; set; }
        public ICollection<TripStop>? TripStops { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Pkcs;

namespace PathWay_Solution.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required,StringLength(maximumLength:50)]
        public string Name { get; set; } = null!;
        public ICollection<Routes>? RoutesFrom { get; set; }
        public ICollection<Routes>? RoutesTo { get; set; }
        public ICollection<TripStop>? TripStops { get; set; }
       
    }

}

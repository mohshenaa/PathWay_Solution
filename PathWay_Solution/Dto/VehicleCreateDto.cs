using PathWay_Solution.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PathWay_Solution.Dto
{
    public class VehicleCreateDto
    {
      //  public string VehicleType { get; set; } = null!;
       
        [Required, StringLength(50)]
        public string VehicleNumber { get; set; } = null!;

        [Required]
        public int Capacity { get; set; }

        [Required]
        public int Doors { get; set; }
        [Required]
        public bool HasAC { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [SwaggerSchema(Nullable = false)]
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

    }
    public class BusCreateDto : VehicleCreateDto
    {

        [Required]
        public int StandingCapacity { get; set; }
    }
    public class MiniBusCreateDto : VehicleCreateDto
    {
        [Required]
        public int StandingCapacity { get; set; }
    }
    public class CarCreateDto : VehicleCreateDto
    {
        [Required]
        public string CarCategory { get; set; } = null!;
    }
    public class MicroCreateDto : VehicleCreateDto
    {
        [Required]
        public string MicroCategory { get; set; } = null!;
    }
}

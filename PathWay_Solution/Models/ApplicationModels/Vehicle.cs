using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public abstract class Vehicle   //for c# oop structure
    {
        [Key]
        public int VehicleId { get; set; }

        [Required,StringLength(50)]
        public string VehicleNumber { get; set; } = null!;
        public int Capacity { get; set; }
        public int Doors { get; set; }
        public bool HasAC { get; set; }
        public string ImageUrl { get; set; } = "";

        [SwaggerSchema(Nullable = false)]
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;
        public ICollection<Trip>? Trips { get; set; }
        public ICollection<VehicleMaintenance>? VehicleMaintenances { get; set; }
    }
    public enum VehicleStatus
    {
        Available,
        OnTrip,
        Maintenance,
        Inactive
    }
    public class Bus : Vehicle
    {
       // public bool HasAC { get; set; }
        public int StandingCapacity { get; set; }
    }
    public class MiniBus : Vehicle
    {
        //  public bool HasAC { get; set; }
        public int StandingCapacity { get; set; }
    }
    public class Car : Vehicle
    {
        public string CarCategory { get; set; } = null!; //differentiate by interior ,model,speed
    }
    public class Micro : Vehicle
    {
        public string MicroCategory { get; set; } = null!; //differentiate by interior ,model,speed
    }
}

namespace PathWay_Solution.Models
{
    public abstract class Vehicle
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = null!;
        public int Capacity { get; set; }

        public string? Status { get; set; } = "Available";

        public ICollection<Trip>? Trips {  get; set; }
        public ICollection<VehicleMaintenance>? VehicleMaintenances {  get; set; }
    }
}

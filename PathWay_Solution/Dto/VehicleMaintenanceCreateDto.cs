namespace PathWay_Solution.Dto
{
    public class VehicleMaintenanceCreateDto
    {
        public int VehicleId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public decimal Cost { get; set; }
    }
    public class VehicleMaintenanceEndDto
    {
        public DateTime EndDate { get; set; }
    }

}

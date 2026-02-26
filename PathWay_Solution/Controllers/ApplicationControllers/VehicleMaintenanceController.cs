using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class VehicleMaintenanceController (PathwayDBContext db): ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await db.VehicleMaintenance
                .Include(a => a.Vehicle)
                .Select(m => new
                {
                    m.VehicleMaintenanceId,
                    m.Description,
                    m.StartDate,
                    m.EndDate,
                    m.Cost,
                    m.Vehicle.VehicleId,
                    m.Vehicle.VehicleNumber
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var maintenance = await db.VehicleMaintenance
                .Include(m => m.Vehicle)
                .FirstOrDefaultAsync(m => m.VehicleMaintenanceId == id);

            if (maintenance == null)
                return NotFound();

            return Ok(maintenance);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetByVehicle(int vehicleId)
        {
            var data = await db.VehicleMaintenance
                .Where(m => m.VehicleId == vehicleId)
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VehicleMaintenanceCreateDto dto)
        {
            var vehicle = await db.Vehicle.FindAsync(dto.VehicleId);
            if (vehicle == null)
                return BadRequest("Vehicle not found");

            // Prevent duplicate active maintenance
            bool alreadyUnderMaintenance = await db.VehicleMaintenance
                .AnyAsync(m => m.VehicleId == dto.VehicleId && m.EndDate == null);

            if (alreadyUnderMaintenance)
                return BadRequest("Vehicle already under maintenance.");

            var maintenance = new VehicleMaintenance
            {
                VehicleId = dto.VehicleId,
                Description = dto.Description,
                StartDate = dto.StartDate,
                Cost = dto.Cost
            };

            // Automatically update vehicle status
            vehicle.Status = VehicleStatus.Available;

            db.VehicleMaintenance.Add(maintenance);
            await db.SaveChangesAsync();

            return Ok("Maintenance record created successfully.");
        }

        [HttpPut("end/{id}")]
        public async Task<IActionResult> EndMaintenance(int id, VehicleMaintenanceEndDto dto)
        {
            var maintenance = await db.VehicleMaintenance
                .Include(m => m.Vehicle)
                .FirstOrDefaultAsync(m => m.VehicleMaintenanceId == id);

            if (maintenance == null)
                return NotFound();

            if (maintenance.EndDate != null)
                return BadRequest("Maintenance already ended.");

            maintenance.EndDate = dto.EndDate;

            // Automatically set vehicle back to Available
             maintenance.Vehicle.Status = VehicleStatus.Available;

            await db.SaveChangesAsync();

            return Ok("Maintenance ended successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var maintenance = await db.VehicleMaintenance.FindAsync(id);

            if (maintenance == null)
                return NotFound();

            if (maintenance.EndDate == null)
                return BadRequest("Cannot delete active maintenance record.");

            db.VehicleMaintenance.Remove(maintenance);
            await db.SaveChangesAsync();

            return Ok("Maintenance record deleted.");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class SeatController(PathwayDBContext db) : ControllerBase
    {

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetSeatsByVehicle(int vehicleId)
        {
            var vehicle = await db.Vehicle
                .Include(v => v.Seats)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle == null)
                return NotFound("Vehicle not found");

            // for car and micro no seat layout needed
            var vehicleType = vehicle.GetType().Name;

            if (vehicleType == nameof(Car) || vehicleType == nameof(Micro))
            {
                return Ok(new
                {
                    Message = "This vehicle type does not use seat layout",
                    vehicle.VehicleId,
                    vehicle.VehicleNumber,
                    vehicle.Capacity
                });
            }

            var seatLayout = vehicle.Seats
                .Select(s => new
                {
                    s.SeatId,
                    s.SeatNumber,
                    s.Row,
                    s.Column,
                    s.IsWindow,
                    s.IsAisle
                })
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Column)
                .ToList();

            return Ok(seatLayout);
        }

        //singel seat info
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeatById(int id)
        {
            var seat = await db.Seat
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.SeatId == id);

            if (seat == null)
                return NotFound("Seat not found");

            return Ok(new
            {
                seat.SeatId,
                seat.SeatNumber,
                seat.Row,
                seat.Column,
                seat.IsWindow,
                seat.IsAisle,
                seat.VehicleId,
                VehicleNumber = seat.Vehicle.VehicleNumber
            });
        }

        //Admin can delete seat (rarely needed)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            var seat = await db.Seat.FindAsync(id);
            if (seat == null)
                return NotFound("Seat not found");

            db.Seat.Remove(seat);
            await db.SaveChangesAsync();

            return Ok("Seat deleted successfully");
        }
    }


}
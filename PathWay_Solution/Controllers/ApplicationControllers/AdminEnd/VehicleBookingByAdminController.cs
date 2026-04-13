using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Controllers.ApplicationControllers.AdminEnd
{
    [Route("api/Admin/VehicleBooking")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class VehicleBookingByAdminController(PathwayDBContext db) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateBookingByAdmin(CreateVehicleBookingByAdminDto dto)
        {
            if (dto.DropTime <= dto.PickupTime)
                return BadRequest("Invalid time range.");

            var vehicle = await db.Vehicle.FindAsync(dto.VehicleId);

            if (vehicle == null)
                return NotFound("Vehicle not found.");

            if (vehicle is not Car && vehicle is not Micro)
                return BadRequest("Only Car or Micro allowed.");

            if (dto.Price <= 0)
                return BadRequest("Invalid price.");

            var isBooked = await db.VehicleBookings.AnyAsync(vb =>
                vb.VehicleId == dto.VehicleId &&
                vb.Status != BookingStatus.Cancelled &&
                vb.PickupTime < dto.DropTime &&
                vb.DropTime > dto.PickupTime
            );

            if (isBooked)
                return BadRequest("Vehicle already booked for this time range.");

            var booking = new VehicleBooking
            {
                VehicleId = dto.VehicleId,
                AppUserId = dto.AppUserId,
                PickupLocation = dto.PickupLocation,
                DropLocation = dto.DropLocation,
                PickupTime = dto.PickupTime,
                DropTime = dto.DropTime,
                Price = dto.Price,
                Status = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Unpaid
            };

            db.VehicleBookings.Add(booking);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Booking created by admin"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await db.VehicleBookings
                .Include(vb => vb.Vehicle)
                .Select(vb => new VehicleBookingDto
                {
                    VehicleBookingId = vb.VehicleBookingId,
                    AppUserId = vb.AppUserId,
                    VehicleType = vb.Vehicle.GetType().Name,
                    VehicleNumber = vb.Vehicle.VehicleNumber,
                    PickupLocation = vb.PickupLocation,
                    DropLocation = vb.DropLocation,
                    PickupTime = vb.PickupTime,
                    DropTime = vb.DropTime,
                    Price = vb.Price,
                    Status = vb.Status.ToString()
                })
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await db.VehicleBookings
                .Include(vb => vb.Vehicle)
                .FirstOrDefaultAsync(vb => vb.VehicleBookingId == id);

            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                return BadRequest("Cannot cancel this booking.");

            booking.Status = BookingStatus.Cancelled;

            if (booking.Vehicle != null)
                booking.Vehicle.Status = VehicleStatus.Available;

            await db.SaveChangesAsync();

            return Ok("Booking cancelled");
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateBookingStatusDto dto)
        {
            var booking = await db.VehicleBookings
                .Include(vb => vb.Vehicle)
                .FirstOrDefaultAsync(vb => vb.VehicleBookingId == id);

            if (booking == null)
                return NotFound();

            if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                return BadRequest("Cannot update final state.");

            bool isValid = booking.Status switch
            {
                BookingStatus.Pending => dto.Status == BookingStatus.Confirmed || dto.Status == BookingStatus.Cancelled,
                BookingStatus.Confirmed => dto.Status == BookingStatus.Ongoing || dto.Status == BookingStatus.Cancelled,
                BookingStatus.Ongoing => dto.Status == BookingStatus.Completed,
                _ => false
            };

            if (!isValid)
                return BadRequest("Invalid transition.");

            booking.Status = dto.Status;

            if (booking.Vehicle != null)
            {
                if (dto.Status == BookingStatus.Ongoing)
                    booking.Vehicle.Status = VehicleStatus.OnTrip;
                else if (dto.Status == BookingStatus.Completed || dto.Status == BookingStatus.Cancelled)
                    booking.Vehicle.Status = VehicleStatus.Available;
            }

            await db.SaveChangesAsync();

            return Ok("Status updated");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;
using System.Security.Claims;

[Route("api/VehicleBooking")]
[ApiController]
public class VehicleBookingController(PathwayDBContext db) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateVehicleBookingDto dto)
    {
        if (dto.DropTime <= dto.PickupTime)
            return BadRequest("Drop time must be after pickup time.");

        var vehicle = await db.Vehicle.FindAsync(dto.VehicleId);

        if (vehicle == null)
            return NotFound("Vehicle not found.");

        if (vehicle is not Car && vehicle is not Micro)
            return BadRequest("Only Car or Micro can be booked.");

        if (vehicle.Status != VehicleStatus.Available)
            return BadRequest("Vehicle is not available.");

        var isBooked = await db.VehicleBookings.AnyAsync(vb =>
            vb.VehicleId == dto.VehicleId &&
            vb.Status != BookingStatus.Cancelled &&
            vb.PickupTime < dto.DropTime &&
            vb.DropTime > dto.PickupTime
        );

        if (isBooked)
            return BadRequest("Vehicle already booked for this time range.");

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized();

        if (!Guid.TryParse(userIdString, out Guid userId))
            return BadRequest("Invalid user ID.");

        var hours = (dto.DropTime - dto.PickupTime).TotalHours;

        decimal ratePerHour = vehicle switch
        {
            Car => 200,
            Micro => 300,
            _ => 0
        };

        decimal price = (decimal)hours * ratePerHour;

        var booking = new VehicleBooking
        {
            VehicleId = dto.VehicleId,
            AppUserId = userId,
            PickupLocation = dto.PickupLocation,
            DropLocation = dto.DropLocation,
            PickupTime = dto.PickupTime,
            DropTime = dto.DropTime,
            Price = price,
            Status = BookingStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid
        };

        db.VehicleBookings.Add(booking);
        await db.SaveChangesAsync();

        return Ok(new
        {
            message = "Booking created successfully",
            bookingId = booking.VehicleBookingId,
            price
        });
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBookingsByUser(Guid userId)
    {
        var bookings = await db.VehicleBookings
            .Include(vb => vb.Vehicle)
            .Where(vb => vb.AppUserId == userId)
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
}
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;

namespace PathWay_Solution.Services
{
    public interface ITripService
    {
        Task<TripResponseDto> CreateTripAsync(TripCreateDto dto); 
        Task<IEnumerable<Trip>> GetAllTripsAsync();
        Task<Trip?> GetTripByIdAsync(int id);
        Task<TripResponseDto?> UpdateTripAsync(int id, TripUpdateDto dto);
        Task<bool> DeleteTripAsync(int id);
        Task<bool> StartTripAsync(int tripId);
        Task<bool> CompleteTripAsync(int tripId);
        Task<bool> CancelTripAsync(int tripId);
    }

    public class TripService(PathwayDBContext db) : ITripService
    {
        //for seat layout like a1 a2 a3 a4 b1 b2....
        private List<Seat> GenerateBusSeats(int tripId, int capacity)
        {
            int seatsPerRow = 4;
            int currentSeat = 0;
            char rowLetter = 'A';

            var seats = new List<Seat>();

            while (currentSeat < capacity)
            {
                for (int col = 1; col <= seatsPerRow && currentSeat < capacity; col++)
                {
                    bool isLeftSide = col <= 2;
                    bool isWindow = col == 1 || col == 4;
                    bool isAisle = col == 2 || col == 3;

                    seats.Add(new Seat
                    {
                        TripId = tripId,
                        Row = rowLetter.ToString(),
                        Column = col,
                        SeatNumber = $"{rowLetter}{col}",
                        IsWindow = isWindow,
                        IsAisle = isAisle,
                        IsBooked = false
                    });

                    currentSeat++;
                }

                rowLetter++;
            }

            return seats;
        }


        public async Task<TripResponseDto> CreateTripAsync(TripCreateDto dto) 
        {
            // all parents cls validation to find out it exists or not
            if (!await db.Routes.AnyAsync(a => a.RouteId == dto.RouteId))
                throw new Exception("Route not found");

            if (!await db.TripSchedule.AnyAsync(a => a.TripScheduleId == dto.TripScheduleId))
                throw new Exception("TripSchedule not found");

            var vehicle = await db.Vehicle.FirstOrDefaultAsync(a => a.VehicleId == dto.VehicleId);
            if (vehicle == null)
                throw new Exception("Vehicle not found");

            if (!await db.Driver.AnyAsync(a => a.DriverId == dto.DriverId))
                throw new Exception("Driver not found");

            if (dto.HelperId.HasValue &&
                !await db.Helper.AnyAsync(a => a.HelperId == dto.HelperId))
                throw new Exception("Helper not found");

            // Time validation
            if (dto.ArrivalTime <= dto.DepartureTime)
                throw new Exception("Arrival must be after departure");

            // Driver availability
            bool driverOverlap = await db.Trip.AnyAsync(t =>
                t.DriverId == dto.DriverId &&
                t.TripStatus != TripStatus.Cancelled &&
                dto.DepartureTime < t.ArrivalTime &&   //New Trip Start < Existing Trip End &&
                 dto.ArrivalTime > t.DepartureTime);   //New Trip End > Existing Trip Start


            if (driverOverlap)
                throw new Exception("Driver already assigned to another trip");

            // Vehicle availability
            bool vehicleOverlap = await db.Trip.AnyAsync(t =>
                t.VehicleId == dto.VehicleId &&
                t.TripStatus != TripStatus.Cancelled &&
                dto.DepartureTime < t.ArrivalTime &&
                dto.ArrivalTime > t.DepartureTime);

            if (vehicleOverlap)
                throw new Exception("Vehicle already assigned to another trip");

            // Helper availability
            if (dto.HelperId.HasValue)
            {
                bool helperOverlap = await db.Trip.AnyAsync(t =>
                    t.HelperId == dto.HelperId &&
                    t.TripStatus != TripStatus.Cancelled &&
                    dto.DepartureTime < t.ArrivalTime &&
                    dto.ArrivalTime > t.DepartureTime);

                if (helperOverlap)
                    throw new Exception("Helper already assigned to another trip");
            }

            var trip = new Trip
            {
                RouteId = dto.RouteId,
                TripScheduleId = dto.TripScheduleId,
                VehicleId = dto.VehicleId,
                DriverId = dto.DriverId,
                HelperId = dto.HelperId,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                TripType = dto.TripType,
                TripStatus = TripStatus.Scheduled
            };

            db.Trip.Add(trip);
            await db.SaveChangesAsync();

            var seats = GenerateBusSeats(trip.TripId, vehicle.Capacity);
            await db.Seat.AddRangeAsync(seats);
            await db.SaveChangesAsync();

            return new TripResponseDto
            {
                TripId = trip.TripId,
                RouteId = trip.RouteId,
                VehicleId = trip.VehicleId,
                DriverId = trip.DriverId,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                TripType = trip.TripType,
                TripStatus = trip.TripStatus
            };
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            return await db.Trip
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .Include(t => t.TripSchedule)
                .ToListAsync();
        }

        public async Task<Trip?> GetTripByIdAsync(int id)
        {
            return await db.Trip
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .Include(t => t.TripSchedule)
                .FirstOrDefaultAsync(t => t.TripId == id);
        }

        public async Task<TripResponseDto?> UpdateTripAsync(int id, TripUpdateDto dto)
        {
            //Using keyword Auto dispose &rollback
            using var transaction = await db.Database.BeginTransactionAsync();  //Start a safe zone. Don’t permanently save anything yet (BeginTransactionAsync())

            var existing = await db.Trip
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.TripId == id);

            if (existing == null)
                return null;

            // Prevent editing completed trips
            if (existing.TripStatus == TripStatus.Completed)
                throw new Exception("Completed trips cannot be edited.");

            // Validate time
            if (dto.ArrivalTime <= dto.DepartureTime)
                throw new Exception("Arrival time must be after departure time.");

            // Driver overlap check
            bool driverOverlap = await db.Trip.AnyAsync(t =>
                t.TripId != id &&
                t.DriverId == dto.DriverId &&
                t.TripStatus != TripStatus.Cancelled &&
                dto.DepartureTime < t.ArrivalTime &&
                dto.ArrivalTime > t.DepartureTime);

            if (driverOverlap)
                throw new Exception("Driver already assigned to another trip.");

            // Helper overlap check (if exists)
            if (dto.HelperId.HasValue)
            {
                bool helperOverlap = await db.Trip.AnyAsync(t =>
                    t.TripId != id &&
                    t.HelperId == dto.HelperId &&
                    t.TripStatus != TripStatus.Cancelled &&
                    dto.DepartureTime < t.ArrivalTime &&
                    dto.ArrivalTime > t.DepartureTime);

                if (helperOverlap)
                    throw new Exception("Helper already assigned to another trip.");
            }

            // Vehicle overlap check
            bool vehicleOverlap = await db.Trip.AnyAsync(t =>
                t.TripId != id &&
                t.VehicleId == dto.VehicleId &&
                t.TripStatus != TripStatus.Cancelled &&
                dto.DepartureTime < t.ArrivalTime &&
                dto.ArrivalTime > t.DepartureTime);

            if (vehicleOverlap)
                throw new Exception("Vehicle already assigned to another trip.");

            // Check if vehicle changed
            bool vehicleChanged = existing.VehicleId != dto.VehicleId;

            if (vehicleChanged)
            {
                // Check if any bookings exist
                bool hasBookings = await db.Booking.AnyAsync(b => b.TripId == id);

                if (hasBookings)
                    throw new Exception("Cannot change vehicle after bookings exist.");

                // Remove old seats
                var oldSeats = await db.Seat
                    .Where(s => s.TripId == id)
                    .ToListAsync();

                db.Seat.RemoveRange(oldSeats);

                // Get new vehicle capacity
                var newVehicle = await db.Vehicle
                    .FirstOrDefaultAsync(v => v.VehicleId == dto.VehicleId);

                if (newVehicle == null)
                    throw new Exception("Selected vehicle not found.");

                db.Seat.RemoveRange(oldSeats);

                var newSeats = GenerateBusSeats(id, newVehicle.Capacity);
                await db.Seat.AddRangeAsync(newSeats);
            }

            // Update Trip Fields
            existing.RouteId = dto.RouteId;
            existing.DriverId = dto.DriverId;
            existing.VehicleId = dto.VehicleId;
            existing.HelperId = dto.HelperId;
            existing.DepartureTime = dto.DepartureTime;
            existing.ArrivalTime = dto.ArrivalTime;
            existing.TripType = dto.TripType;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();   //Everything successful. Now permanently save(CommitAsync())

            return new TripResponseDto
            {
                TripId = existing.TripId,
                RouteId = existing.RouteId,
                VehicleId = existing.VehicleId,
                DriverId = existing.DriverId,
                DepartureTime = existing.DepartureTime,
                ArrivalTime = existing.ArrivalTime,
                TripType = existing.TripType,
                TripStatus = existing.TripStatus,
                StartTime = existing.StartTime,
                EndTime = existing.EndTime
            };
        }

        public async Task<bool> DeleteTripAsync(int id)
        {
            var trip = await db.Trip.FindAsync(id);
            if (trip == null) return false;

            db.Trip.Remove(trip);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StartTripAsync(int tripId)
        {
            var trip = await db.Trip.FindAsync(tripId);

            if (trip == null)
                return false;

            switch (trip.TripStatus)    // State Machine Validation
            {
                case TripStatus.Scheduled:
                    trip.TripStatus = TripStatus.Running;
                     trip.StartTime = DateTime.UtcNow;
                    break;

                case TripStatus.Running:
                case TripStatus.Completed:
                case TripStatus.Cancelled:
                default:
                    return false;
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteTripAsync(int tripId)
        {
            var trip = await db.Trip.FindAsync(tripId);

            if (trip == null)
                return false;

            switch (trip.TripStatus)
            {
                case TripStatus.Running:
                    trip.TripStatus = TripStatus.Completed;
                    trip.EndTime = DateTime.UtcNow;
                    break;

                case TripStatus.Scheduled:
                case TripStatus.Completed:
                case TripStatus.Cancelled:
                default:
                    return false;
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelTripAsync(int tripId)
        {
            var trip = await db.Trip.FindAsync(tripId);

            if (trip == null)
                return false;

            switch (trip.TripStatus)
            {
                case TripStatus.Scheduled:
                    trip.TripStatus = TripStatus.Cancelled;
                    trip.CancelledTime = DateTime.UtcNow;
                    break;

                case TripStatus.Running:
                case TripStatus.Completed:
                case TripStatus.Cancelled:
                default:
                    return false;
            }

            await db.SaveChangesAsync();
            return true;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;

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

    public class TripService : ITripService
    {
        private readonly PathwayDBContext _db;

        public TripService(PathwayDBContext db)
        {
            _db = db;
        }

        //// Generate seat availability per trip (based on vehicle capacity)
        //private List<Seat> GenerateTripSeats(int tripId, int vehicleId)
        //{
        //    var vehicle = _db.Vehicle.Find(vehicleId);
        //    if (vehicle == null) throw new Exception("Vehicle not found for seat generation");

        //    int capacity = vehicle.Capacity;
        //    int seatsPerRow = 4;
        //    char rowLetter = 'A';
        //    int currentSeat = 0;
        //    var seats = new List<Seat>();

        //    while (currentSeat < capacity)
        //    {
        //        for (int col = 1; col <= seatsPerRow && currentSeat < capacity; col++)
        //        {
        //            bool isWindow = col == 1 || col == 4;
        //            bool isAisle = col == 2 || col == 3;

        //            seats.Add(new Seat
        //            {
        //                TripId = tripId,
        //                VehicleId = vehicleId,
        //                Row = rowLetter.ToString(),
        //                Column = col,
        //                SeatNumber = $"{rowLetter}{col}",
        //                IsWindow = isWindow,
        //                IsAisle = isAisle,
        //                IsBooked = false
        //            });

        //            currentSeat++;
        //        }
        //        rowLetter++;
        //    }

        //    return seats;
        //}

        public async Task<TripResponseDto> CreateTripAsync(TripCreateDto dto)
        {
            // Validate existence of Route, Schedule, Vehicle, Driver, Helper
            if (!await _db.Routes.AnyAsync(r => r.RouteId == dto.RouteId))
                throw new Exception("Route not found");

            if (!await _db.TripSchedule.AnyAsync(s => s.TripScheduleId == dto.TripScheduleId))
                throw new Exception("Trip schedule not found");

            var vehicle = await _db.Vehicle.FindAsync(dto.VehicleId);
            if (vehicle == null) throw new Exception("Vehicle not found");

            var vehicleType = vehicle.GetType().Name;

            //car and micro is only for rent
            if ((vehicleType == nameof(Car) || vehicleType == nameof(Micro))
                && dto.TripType != TripType.OnDemand)
            {
                throw new Exception("Car and Micro can only be used for OnDemand trips");
            }

            //minibus is only for rent or local
            if (vehicleType == nameof(MiniBus) && dto.TripType == TripType.OnDemand)
            {
                // allowed → do nothing
            }
            if (vehicleType == nameof(MiniBus) && dto.IsExpress)
            {
                throw new Exception("MiniBus cannot be used for Express trips");
            }

            if (!await _db.Driver.AnyAsync(d => d.DriverId == dto.DriverId))
                throw new Exception("Driver not found");

            if (dto.HelperId.HasValue && !await _db.Helper.AnyAsync(h => h.HelperId == dto.HelperId))
                throw new Exception("Helper not found");

            // Validate times
            if (dto.ArrivalTime <= dto.DepartureTime)
                throw new Exception("Arrival must be after departure");

            // Check overlapping trips
            bool driverOverlap = await _db.Trip.AnyAsync(t =>
                t.DriverId == dto.DriverId && t.TripStatus != TripStatus.Cancelled && t.TripStatus != TripStatus.Completed &&
                dto.DepartureTime < t.ArrivalTime && dto.ArrivalTime > t.DepartureTime);

            if (driverOverlap) throw new Exception("Driver already assigned to another trip");

            bool vehicleOverlap = await _db.Trip.AnyAsync(t =>
                t.VehicleId == dto.VehicleId && t.TripStatus != TripStatus.Cancelled && t.TripStatus != TripStatus.Completed &&
                dto.DepartureTime < t.ArrivalTime && dto.ArrivalTime > t.DepartureTime);

            if (vehicleOverlap) throw new Exception("Vehicle already assigned to another trip");

            bool helperOverlap = false;
            if (dto.HelperId.HasValue)
            {
                helperOverlap = await _db.Trip.AnyAsync(t =>
                    t.HelperId == dto.HelperId && t.TripStatus != TripStatus.Cancelled && t.TripStatus != TripStatus.Completed &&
                    dto.DepartureTime < t.ArrivalTime && dto.ArrivalTime > t.DepartureTime);
                if (helperOverlap) throw new Exception("Helper already assigned to another trip");
            }

            var trip = new Trip
            {
                RouteId = dto.RouteId,
                TripScheduleId = dto.TripScheduleId,
                VehicleId = dto.VehicleId,
                DriverId = dto.DriverId,
                HelperId = dto.HelperId,
                IsExpress=false,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                TripType = dto.TripType,
                TripStatus = TripStatus.Scheduled
            };

            _db.Trip.Add(trip);
            await _db.SaveChangesAsync();

            var seats = await _db.Seat
     .Where(s => s.VehicleId == trip.VehicleId)
     .ToListAsync();

            var tripSeats = seats.Select(s => new TripSeat
            {
                TripId = trip.TripId,
                SeatId = s.SeatId,
                IsBooked = false,
                IsLocked = false,
                LockedUntil = null
            }).ToList();

            await _db.TripSeat.AddRangeAsync(tripSeats);
            await _db.SaveChangesAsync();

            // Optional: Create TripStops for express trips (1-2 stops)
            if (trip.TripType == TripType.Scheduled)
            {
                var stops = new List<TripStop>();
                if (trip.Route.FromLocationId != trip.Route.ToLocationId)
                {
                    stops.Add(new TripStop
                    {
                        TripId = trip.TripId,
                        LocationId = trip.Route.FromLocationId,
                        StopOrder = 1,
                        BreakDurationMinutes = 15
                    });
                    stops.Add(new TripStop
                    {
                        TripId = trip.TripId,
                        LocationId = trip.Route.ToLocationId,
                        StopOrder = 2,
                        BreakDurationMinutes = 15
                    });
                }
                await _db.TripStop.AddRangeAsync(stops);
                await _db.SaveChangesAsync();
            }

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
            return await _db.Trip
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .Include(t => t.TripSchedule)
                .Include(t => t.TripStops)
                .ToListAsync();
        }

        public async Task<Trip?> GetTripByIdAsync(int id)
        {
            return await _db.Trip
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .Include(t => t.TripSchedule)
                .Include(t => t.TripStops)
                .Include(t => t.TripSeat)
    .ThenInclude(ts => ts.Seat)
                .FirstOrDefaultAsync(t => t.TripId == id);

        }

        public async Task<TripResponseDto?> UpdateTripAsync(int id, TripUpdateDto dto)
        {
            var trip = await _db.Trip.FindAsync(id);
            if (trip == null) return null;

            if (trip.TripStatus == TripStatus.Completed)
                throw new Exception("Cannot edit completed trip");

            // Update fields
            trip.RouteId = dto.RouteId;
            trip.TripScheduleId = dto.TripScheduleId;
            trip.VehicleId = dto.VehicleId;
            trip.DriverId = dto.DriverId;
            trip.HelperId = dto.HelperId;
            trip.DepartureTime = dto.DepartureTime;
            trip.ArrivalTime = dto.ArrivalTime;
            trip.TripType = dto.TripType;

            await _db.SaveChangesAsync();

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

        public async Task<bool> DeleteTripAsync(int id)
        {
            var trip = await _db.Trip.FindAsync(id);
            if (trip == null) return false;

            //// Remove seats
            //var seats = _db.Seat.Where(s => s.TripSeatId == id);
            //_db.Seat.RemoveRange(seats);

            // Remove stops
            var stops = _db.TripStop.Where(s => s.TripId == id);
            _db.TripStop.RemoveRange(stops);

            _db.Trip.Remove(trip);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StartTripAsync(int tripId)
        {
            var trip = await _db.Trip.FindAsync(tripId);
            if (trip == null) return false;
            if (trip.TripStatus != TripStatus.Scheduled) return false;

            trip.TripStatus = TripStatus.Running;
            trip.StartTime = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteTripAsync(int tripId)
        {
            var trip = await _db.Trip.FindAsync(tripId);
            if (trip == null) return false;
            if (trip.TripStatus != TripStatus.Running) return false;

            trip.TripStatus = TripStatus.Completed;
            trip.EndTime = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelTripAsync(int tripId)
        {
            var trip = await _db.Trip.FindAsync(tripId);
            if (trip == null) return false;
            if (trip.TripStatus != TripStatus.Scheduled) return false;

            trip.TripStatus = TripStatus.Cancelled;
            trip.CancelledTime = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
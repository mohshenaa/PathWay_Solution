using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Models;

namespace PathWay_Solution.Services
{
    public interface ITripService
    {
        Task<Trip> CreateTripAsync(Trip trip);
        Task<IEnumerable<Trip>> GetAllTripsAsync();
        Task<Trip?> GetTripByIdAsync(int id);
        Task<Trip?> UpdateTripAsync(int id, Trip trip);
        Task<bool> DeleteTripAsync(int id);
        Task<bool> StartTripAsync(int tripId);
        Task<bool> CompleteTripAsync(int tripId);
        Task<bool> CancelTripAsync(int tripId);
    }

    public class TripService(PathwayDBContext db) : ITripService
    {
      
        public async Task<Trip> CreateTripAsync(Trip trip)
        {
            // TODO: validation + availability + save + seat generation
            throw new NotImplementedException();
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

        public async Task<Trip?> UpdateTripAsync(int id, Trip trip)
        {
            var existing = await db.Trip.FindAsync(id);
            if (existing == null) return null;

            // TODO: business rules validation

            existing.RouteId = trip.RouteId;
            existing.DriverId = trip.DriverId;
            existing.VehicleId = trip.VehicleId;
            existing.HelperId = trip.HelperId;
            existing.DepartureTime = trip.DepartureTime;
            existing.ArrivalTime = trip.ArrivalTime;
            existing.TripType = trip.TripType;

            await db.SaveChangesAsync();
            return existing;
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
            if (trip == null) return false;

            if (trip.TripStatus != TripStatus.Scheduled) return false;

            trip.TripStatus = TripStatus.Running;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteTripAsync(int tripId)
        {
            var trip = await db.Trip.FindAsync(tripId);
            if (trip == null) return false;

            if (trip.TripStatus != TripStatus.Running) return false;

            trip.TripStatus = TripStatus.Completed;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelTripAsync(int tripId)
        {
            var trip = await db.Trip.FindAsync(tripId);
            if (trip == null) return false;

            trip.TripStatus = TripStatus.Cancelled;
            await db.SaveChangesAsync();
            return true;
        }
    }
}

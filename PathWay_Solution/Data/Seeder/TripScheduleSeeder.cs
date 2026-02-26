using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Data.Seeder
{
    public class TripScheduleSeeder
    {
        public static async Task SeedAsync(PathwayDBContext db)
        {
          
            if (!await db.TripSchedule.AnyAsync())
            {
                var routeDhakaCtg = await db.Routes
                    .Include(r => r.FromLocation)
                    .Include(r => r.ToLocation)
                    .FirstOrDefaultAsync(r => r.FromLocation.Name == "Dhaka"
                                           && r.ToLocation.Name == "Chattogram");
                
                var routeDhakaNoakhali = await db.Routes
                    .Include(r => r.FromLocation)
                    .Include(r => r.ToLocation)
                    .FirstOrDefaultAsync(r => r.FromLocation.Name == "Dhaka"
                                           && r.ToLocation.Name == "Noakhali");

                if (routeDhakaCtg != null)
                {
                    var schedule1 = new TripSchedule
                    {
                        RouteId = routeDhakaCtg.RouteId,
                        VehicleType = "Bus",
                        StartTime = new TimeSpan(6, 0, 0),
                        FrequencyHours = 3,
                        Direction = "Dhaka to Chattogram",
                        IsActive = true
                    };

                    var schedule2 = new TripSchedule
                    {
                        RouteId = routeDhakaCtg.RouteId,
                        VehicleType = "Bus",
                        StartTime = new TimeSpan(9, 0, 0),
                        FrequencyHours = 3,
                        Direction = "Chattogram to Dhaka",
                        IsActive = true
                    };

                    db.TripSchedule.AddRange(schedule1, schedule2);
                }

                if (routeDhakaNoakhali != null)
                {
                    var schedule3 = new TripSchedule
                    {
                        RouteId = routeDhakaNoakhali.RouteId,
                        VehicleType = "Mini Bus",
                        StartTime = new TimeSpan(7, 30, 0),
                        FrequencyHours = 2,
                        Direction = "Dhaka to Noakhali",
                        IsActive = true
                    };

                    db.TripSchedule.Add(schedule3);
                }

                await db.SaveChangesAsync();
            }
        }
    }
}
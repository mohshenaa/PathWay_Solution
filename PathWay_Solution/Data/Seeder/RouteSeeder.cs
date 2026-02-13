using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Models;
using System.Linq; 

namespace PathWay_Solution.Data.Seeder
{
    public class RouteSeeder
    {
        public static async Task SeedAsync(PathwayDBContext db)
        {
            if (!await db.Routes.AnyAsync())
            {
                var locations = await db.Location.ToListAsync();

                var from = await db.Location
                     .FirstAsync(l => l.Name == "Dhaka");

                var to = await db.Location
                    .FirstAsync(l => l.Name == "Chittagong");

                var route = new Routes
                {
                    FromLocationId = from.LocationId,
                    ToLocationId = to.LocationId,
                    DistanceInKm = 250,
                    IsActive = true
                };

                db.Routes.Add(route);
                await db.SaveChangesAsync();
            }
        }
    }
}

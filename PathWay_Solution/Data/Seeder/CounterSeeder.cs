using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Models.ApplicationModels;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Data.Seeder
{
    public class CounterSeeder
    {
        public static async Task SeedAsync(PathwayDBContext db)
        {
            if (!await db.Counters.AnyAsync())
            {
                var route = await db.Routes
                 .OrderBy(r => r.RouteId)
                 .FirstAsync();

                var counter = new Counters
                {
                    RouteId = route.RouteId,
                    CounterName = "Main Counter",
                    Address = "Head Office",
                    ContactNumber = "01700000000"
                };
                db.Counters.Add(counter);
                await db.SaveChangesAsync();
            }
        }
    }
}

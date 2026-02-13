using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Models;

namespace PathWay_Solution.Data.Seeder
{
    public class LocationSeeder
    {
        public static async Task SeedAsync(PathwayDBContext db)
        {
            if (!await db.Location.AnyAsync())
            {
                db.Location.AddRange(
                    new Location { Name = "Dhaka" },
                    new Location { Name = "Chittagong" }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using PathWay_Solution.IdentityModels;

namespace PathWay_Solution.Data.Seeder
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<AppRole> roleManager)
        {
            string[] roles = { "Admin", "Driver", "Passenger", "CounterStaff" };

            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole
                    {
                        Name = role,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow
                    });
                }
            }

        }
    }
}

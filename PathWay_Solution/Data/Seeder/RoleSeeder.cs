using Microsoft.AspNetCore.Identity;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Data.Seeder
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Passenger = "Passenger";
        public const string Driver = "Driver";
        public const string CounterStaff = "CounterStaff";
    }
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<AppRole> roleManager)
        {
            string[] roles =
            {
            AppRoles.Admin,
            AppRoles.Driver,
            AppRoles.Passenger,
            AppRoles.CounterStaff
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
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

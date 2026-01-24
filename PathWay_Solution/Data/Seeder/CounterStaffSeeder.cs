using Microsoft.AspNetCore.Identity;
using PathWay_Solution.IdentityModels;

namespace PathWay_Solution.Data.Seeder
{
    public class CounterStaffSeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> usermanager)
        {
            var staffEmail = "staff@gmail.com";
            var staffPass = "Staff@123";
            var staffUser = await usermanager.FindByEmailAsync(staffEmail);

            if (staffUser == null)
            {
                staffUser = new AppUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FirstName = "Counter",
                    LastName = "Staff",
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedOn = DateTime.UtcNow
                };

                var result = await usermanager.CreateAsync(staffUser,staffPass);

                if (!result.Succeeded)
                {
                    throw new Exception("Counter staff user creation failed");
                }
            }

            if (!await usermanager.IsInRoleAsync(staffUser, "CounterStaff"))
            {
                await usermanager.AddToRoleAsync(staffUser, "CounterStaff");
            }

        }
    }
}

using Microsoft.AspNetCore.Identity;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Data.Seeder
{
    public class AdminUserSeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> usermanager)
        {
            var adminEmail = "admin@gmail.com";
            var adminPass = "Admin@123";
            var adminUser = await usermanager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName= adminEmail,
                    Email= adminEmail,
                    FirstName= "Admin",
                    LastName="Pannel",
                    IsActive= true,
                    EmailConfirmed=true,
                    CreatedOn= DateTime.UtcNow
                };

                var result= await usermanager.CreateAsync(adminUser,adminPass);

                if (!result.Succeeded)
                {
                    throw new Exception("Admin user creation failed");
                }
            }

            if (!await usermanager.IsInRoleAsync(adminUser,"Admin"))
            {
                await usermanager.AddToRoleAsync(adminUser, "Admin");
            }

        }
    }
}

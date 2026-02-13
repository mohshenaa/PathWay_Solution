using Microsoft.AspNetCore.Identity;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Data.Seeder
{
    public class AdminUserSeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager, PathwayDBContext db)
        {
            var roleName = "Admin";
            var adminEmail = "admin@gmail.com";
            var adminPass = "Admin@123";
           
            //check role exists
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(
                    new AppRole { Name = roleName });
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

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

                var result= await userManager.CreateAsync(adminUser,adminPass);

                if (!result.Succeeded)
                {
                    throw new Exception("Admin user creation failed");
                }
                await userManager.AddToRoleAsync(adminUser, roleName);
            }

            if (!await userManager.IsInRoleAsync(adminUser,"Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

        }
    }
}

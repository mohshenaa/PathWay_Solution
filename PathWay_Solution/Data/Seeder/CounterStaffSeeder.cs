using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Data.Seeder
{
    public class CounterStaffSeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager, PathwayDBContext db)
        {
            var roleName = "CounterStaff";
            var staffEmail = "staff@gmail.com";
            var staffPass = "Staff@123";
           
            //check role exists
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(
                    new AppRole{ Name = roleName });
            }

            //check counter exists
            var counter = await db.Counters.FirstOrDefaultAsync();
            if (counter == null)
            {
                await CounterSeeder.SeedAsync(db);
                counter = await db.Counters.FirstAsync();
            }

            //create user
            var staffUser = await userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                staffUser = new AppUser
                {                
                    UserName = staffEmail,
                    Email = staffEmail,
                    FirstName = "Counter",
                    LastName = "Staff",
                    PhoneNumber = "01745676543",
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedOn = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(staffUser, staffPass);

                if (!result.Succeeded)
                {
                    throw new Exception("Counter staff user creation failed");
                }
                await userManager.AddToRoleAsync(staffUser, roleName);
            }

            // create employee if not exists
            var employee = await db.Employee
                .FirstOrDefaultAsync(e => e.AppUserId == staffUser.Id);

            if (employee == null)
            {
                employee = new Employee
                {
                    FirstName = "Counter",
                    LastName = "Staff",
                    PhoneNumber = "01700000000",
                //    EmployeeRole = roleName,
                    IsActive = true,
                    AppUserId = staffUser.Id  
                };

                db.Employee.Add(employee);
                await db.SaveChangesAsync();
            }

            // create counterstaff if not exists
            var existingCounterStaff = await db.CounterStaff
                .FirstOrDefaultAsync(cs => cs.EmployeeId == employee.EmployeeId);

            if (existingCounterStaff == null)
            {
                var counterStaff = new CounterStaff
                {
                    EmployeeId = employee.EmployeeId,
                    CounterId = counter.CounterId ,
                    IsAvailable = true
                };

                db.CounterStaff.Add(counterStaff);
                await db.SaveChangesAsync();
            }

        }
    }
}

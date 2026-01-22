using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.IdentityModels;

namespace PathWay_Solution.Data
{
    public class PathwayDBContext: IdentityDbContext<AppUser,AppRole,Guid>  //IdentityDbContext class inherits from DbContext...Our application Context class needs to be inherited from the IdentityDbContext class instead of the DbContext class. This is because IdentityDbContext inherits from the Entity Framework Core DbContext class and provides all the DbSet properties needed to manage the Identity Tables.
    {
        public PathwayDBContext(DbContextOptions<PathwayDBContext> options):base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ////to rename table name
            //builder.Entity<AppUser>().ToTable("users");

            //seeding
            var adminRoleId = Guid.Parse("abcdefab-0123-4567-8901-abcdef012345");
            builder.Entity<AppRole>().HasData(new AppRole { Id= adminRoleId ,Name="Admin",NormalizedName="ADMIN",Description="Administrator role with full permissions!",IsActive=true,CreatedOn=new DateTime(2026,01,22),ModifiedOn= new DateTime(2026, 01, 22) });
            builder.Entity<AppRole>().HasData(new AppRole { 
                Id= Guid.Parse("01234567-89ab-cdef-0123-456789abcdef") ,Name="CounterStaff",NormalizedName="COUNTERSTAFF",Description="Administrator role with full permissions!",IsActive=true,CreatedOn=new DateTime(2026,01,22),ModifiedOn= new DateTime(2026, 01, 22) });
        }
        public DbSet<Address> Address { get; set; }
    }
}

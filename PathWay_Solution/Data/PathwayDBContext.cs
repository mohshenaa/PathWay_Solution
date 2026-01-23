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

        public DbSet<Address> Address { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ////to rename table name
            //builder.Entity<AppUser>().ToTable("users");

            

            builder.Entity<Address>()
                .HasOne(a => a.AppUser)
                .WithMany(a => a.Address)
                .HasForeignKey(a => a.UserId);

        }
       
    }
}

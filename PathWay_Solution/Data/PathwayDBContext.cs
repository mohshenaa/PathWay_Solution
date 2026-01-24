using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.IdentityModels;
using PathWay_Solution.Models;
using System.Reflection.Emit;

namespace PathWay_Solution.Data
{
    public class PathwayDBContext : IdentityDbContext<AppUser, AppRole, Guid>  //IdentityDbContext class inherits from DbContext...Our application Context class needs to be inherited from the IdentityDbContext class instead of the DbContext class. This is because IdentityDbContext inherits from the Entity Framework Core DbContext class and provides all the DbSet properties needed to manage the Identity Tables.
    {
        public PathwayDBContext(DbContextOptions<PathwayDBContext> options) : base(options)
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


            builder.Entity<Vehicle>()
           .HasDiscriminator<string>("VehicleType")
           .HasValue<Bus>("Bus")
           .HasValue<MiniBus>("MiniBus")
           .HasValue<Car>("Car")
           .HasValue<Micro>("Micro");

            //seed for bus
            builder.Entity<Bus>().HasData(
    new Bus
    {
        VehicleId = 1,
        VehicleNumber = "DHA-BUS-101",
        Capacity = 50,
        Doors = 2,
        HasAC = true,
        StandingCapacity = 20,
        ImageUrl = "/images/vehicles/bus1.png",
        Status = "Available"
    }
);
            //seed for mini bus
            builder.Entity<MiniBus>().HasData(
    new MiniBus
    {
        VehicleId = 2,
        VehicleNumber = "DHA-MINI-201",
        Capacity = 30,
        Doors = 2,
        HasAC = true,
        ImageUrl = "/images/vehicles/minibus1.png",
        Status = "OnTrip"
    }
);
            //seed for car
            builder.Entity<Car>().HasData(
    new Car
    {
        VehicleId = 3,
        VehicleNumber = "DHA-CAR-301",
        Capacity = 5,
        Doors = 4,
        CarCategory = "Sedan",
        ImageUrl = "/images/vehicles/car1.png",
        Status = "Available"
    }
);
            //seed for micro
            builder.Entity<Micro>().HasData(
    new Micro
    {
        VehicleId = 4,
        VehicleNumber = "DHA-MICRO-401",
        Capacity = 8,
        Doors = 4,
        MicroCategory = "Luxury",
        ImageUrl = "/images/vehicles/micro1.png",
        Status = "Maintenance"
    }
);

            //employee
            builder.Entity<Employee>(entity =>
            {
                entity.HasKey(a => a.EmployeeId);

                entity.HasMany(a => a.Salaries)
                .WithOne(a => a.Employee)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //driver
            builder.Entity<Driver>(entity =>
            {
                entity.HasKey(a => a.DriverId);

                entity.HasOne(a => a.Employee)
                .WithOne()
                .HasForeignKey<Driver>(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.Trips)
                .WithOne(a => a.Driver)
                .HasForeignKey(a => a.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //helper
            builder.Entity<Helper>(entity =>
            {
                entity.HasKey(a => a.HelperId);

                entity.HasOne(a => a.Employee)
                .WithOne()
                .HasForeignKey<Helper>(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.Trips)
                .WithOne(a => a.Helper)
                .HasForeignKey(a => a.HelperId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //counterstaff
            builder.Entity<CounterStaff>(entity =>
            {
                entity.HasKey(a => a.CounterStaffId);

                entity.HasOne(a => a.Employee)
               .WithOne()
               .HasForeignKey<CounterStaff>(a => a.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Counters)
               .WithMany(a => a.CounterStaff)
               .HasForeignKey(a => a.CounterId)
               .OnDelete(DeleteBehavior.Restrict);


            });
            //salary
            builder.Entity<Salary>(entity =>
            {
                entity.HasKey(a => a.SalaryId);

                entity.HasOne(a => a.Employee)
                .WithMany(a => a.Salaries)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //trip
            builder.Entity<Trip>(entity =>
            {
                entity.HasKey(a => a.TripId);

                entity.HasOne(a => a.Driver)
                .WithMany(a => a.Trips)
                .HasForeignKey(a => a.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Helper)
                .WithMany(a => a.Trips)
                .HasForeignKey(a => a.HelperId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}

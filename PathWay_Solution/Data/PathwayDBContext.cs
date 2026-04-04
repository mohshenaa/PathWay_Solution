using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;
using PathWay_Solution.Models.IdentityModels;
using System.Reflection.Emit;

namespace PathWay_Solution.Data
{
    public class PathwayDBContext : IdentityDbContext<AppUser, AppRole, Guid>  //IdentityDbContext class inherits from DbContext...Our application Context class needs to be inherited from the IdentityDbContext class instead of the DbContext class. This is because IdentityDbContext inherits from the Entity Framework Core DbContext class and provides all the DbSet properties needed to manage the Identity Tables.
    {
        public PathwayDBContext(DbContextOptions<PathwayDBContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admin { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<CancellationRefund> CancellationRefund { get; set; }
        public DbSet<Counters> Counters { get; set; }
        public DbSet<CounterStaff> CounterStaff { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<Helper> Helper { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Passenger> Passenger { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<ReportAnalytics> ReportAnalytics { get; set; }
        public DbSet<ReviewRating> ReviewRating { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Salary> Salary { get; set; }
        public DbSet<Seat> Seat { get; set; }

        public DbSet<BookingSeat> BookingSeat { get; set; }
        public DbSet<TripSeat> TripSeat { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<TripSchedule> TripSchedule { get; set; }
        public DbSet<TripStop> TripStop { get; set; }
        public DbSet<Vehicle> Vehicle { get; set; }
        public DbSet<VehicleMaintenance> VehicleMaintenance { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ////to rename table name
            //builder.Entity<AppUser>().ToTable("users");

            builder.Entity<TripSeat>();
            builder.Entity<BookingSeat>();

            //address
            builder.Entity<Address>(entity =>
            {
                entity.HasOne(a => a.AppUser)
                .WithMany(a => a.Address)
                .HasForeignKey(a => a.UserId);
            });

            //vehicle
            builder.Entity<Vehicle>(entity =>
            {
                entity.HasMany(a => a.Trips)
                .WithOne(a => a.Vehicle)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(v => v.VehicleNumber)
               .IsUnique();

                entity.Property(v => v.Status)
               .HasConversion<string>();

                entity.HasDiscriminator<string>("VehicleType")
               .HasValue<Bus>("Bus")
               .HasValue<MiniBus>("MiniBus")
               .HasValue<Car>("Car")
               .HasValue<Micro>("Micro");
            });

            //seed for bus
            builder.Entity<Bus>().HasData(
                 new
                 {
                     VehicleId = 1,
                     VehicleNumber = "DHA-BUS-101",
                     Capacity = 50,
                     Doors = 2,
                     HasAC = true,
                     StandingCapacity = 20,
                     ImageUrl = "/images/vehicles/bus1.png",
                     Status = VehicleStatus.Available,
                     VehicleType = "Bus"
                 }
             );
            //seed for mini bus
            builder.Entity<MiniBus>().HasData(
                 new
                 {
                     VehicleId = 2,
                     VehicleNumber = "DHA-MINI-201",
                     Capacity = 30,
                     Doors = 2,
                     HasAC = true,
                     ImageUrl = "/images/vehicles/minibus1.png",
                     StandingCapacity = 10,
                     Status = VehicleStatus.Available,
                     VehicleType = "MiniBus"
                 }
             );
            //seed for car
            builder.Entity<Car>().HasData(
                 new
                 {
                     VehicleId = 3,
                     VehicleNumber = "DHA-CAR-301",
                     Capacity = 5,
                     Doors = 4,
                     HasAC = true,
                     CarCategory = "Sedan",
                     ImageUrl = "/images/vehicles/car1.png",
                     Status = VehicleStatus.Available,
                     VehicleType = "Car"
                 }
             );
            //seed for micro
            builder.Entity<Micro>().HasData(
                 new
                 {
                     VehicleId = 4,
                     VehicleNumber = "DHA-MICRO-401",
                     Capacity = 8,
                     Doors = 4,
                     HasAC = true,
                     MicroCategory = "Luxury",
                     ImageUrl = "/images/vehicles/micro1.png",
                     Status = VehicleStatus.Available,
                     VehicleType = "Micro"
                 }
             );

            //employee
            builder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.AppUser)
                .WithOne()
                .HasForeignKey<Employee>(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(a => a.Salaries)
                .WithOne(a => a.Employee)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //driver
            builder.Entity<Driver>(entity =>
            {

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
                entity.HasOne(a => a.Employee)
                .WithMany(a => a.Salaries)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasIndex(s => new { s.EmployeeId, s.Month, s.Year })
               .IsUnique();
            });


            //trip
            builder.Entity<Trip>(entity =>
            {
                entity.HasOne(a => a.Driver)
                .WithMany(a => a.Trips)
                .HasForeignKey(a => a.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Helper)
                .WithMany(a => a.Trips)
                .HasForeignKey(a => a.HelperId)
                .OnDelete(DeleteBehavior.Restrict);

                //entity.HasOne(a => a.Route)
                //.WithMany(a => a.Trips)
                //.HasForeignKey(a => a.RouteId)
                //.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(a => a.TripSeat)
                .WithOne(a => a.Trip)
                .HasForeignKey(a => a.TripId);

                entity.HasOne(t => t.TripSchedule)
                .WithMany(ts => ts.Trips)
                .HasForeignKey(t => t.TripScheduleId);
            });

            //trip stop
            builder.Entity<TripStop>(entity =>
            {

                entity.HasOne(ts => ts.Route)
               .WithMany(r => r.TripStops)
               .HasForeignKey(ts => ts.RouteId)
              .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Location)
               .WithMany(a => a.TripStops)
               .HasForeignKey(a => a.LocationId)
               .OnDelete(DeleteBehavior.Restrict);
            });

            //seat
            builder.Entity<Seat>(entity =>
            {

                entity.HasOne(s => s.Vehicle)
    .WithMany(v => v.Seats)
    .HasForeignKey(s => s.VehicleId)
    .OnDelete(DeleteBehavior.Restrict);
            });

            //booking
            builder.Entity<Booking>(entity =>
            {

                entity.HasOne(a => a.Passenger)
                .WithMany(a => a.Bookings)
                .HasForeignKey(a => a.PassengerId);

                entity.HasOne(a => a.Trip)
                .WithMany(a => a.Bookings)
                .HasForeignKey(a => a.TripId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //payment
            builder.Entity<Payment>(entity =>
            {

                entity.HasOne(a => a.Booking)
                .WithOne(a => a.Payment)
                .HasForeignKey<Payment>(a => a.BookingId);
            });

            //Cancellation and refund
            builder.Entity<CancellationRefund>(entity =>
            {

                entity.HasOne(a => a.Booking)
                .WithOne(a => a.CancellationRefund)
                .HasForeignKey<CancellationRefund>(a => a.BookingId);
            });


            //vehicle maintenance
            builder.Entity<VehicleMaintenance>(entity =>
            {

                entity.HasOne(a => a.Vehicle)
                .WithMany(a => a.VehicleMaintenances)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            //tripseat
            builder.Entity<TripSeat>(entity =>
            {

                entity.HasOne(ts => ts.Trip)
    .WithMany(t => t.TripSeat)
    .HasForeignKey(ts => ts.TripId)
    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ts => ts.Seat)
    .WithMany()
    .HasForeignKey(ts => ts.SeatId)
    .OnDelete(DeleteBehavior.Restrict);

            });

            //bookingseat
            builder.Entity<BookingSeat>(entity =>
            {

                entity.HasOne(bs => bs.Booking)
               .WithMany(b => b.BookingSeats)
               .HasForeignKey(bs => bs.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bs => bs.TripSeat)
                .WithMany()
                .HasForeignKey(bs => bs.TripSeatId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //routes
            builder.Entity<Routes>(entity =>
            {

                entity.HasOne(a => a.FromLocation)
                .WithMany(a => a.RoutesFrom)
                .HasForeignKey(a => a.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.ToLocation)
                .WithMany(a => a.RoutesTo)
                .HasForeignKey(a => a.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            //;ocation
            builder.Entity<Location>(entity =>
            {
                entity.HasIndex(a => a.Name)
                .IsUnique();
            });

            //review
            builder.Entity<ReviewRating>(entity =>
            {

                entity.HasOne(a => a.Passenger)
                .WithMany(a => a.Reviews)
                .HasForeignKey(a => a.PassengerId);

                entity.HasOne(a => a.Trip)
                .WithMany(a => a.ReviewRating)
                .HasForeignKey(a => a.TripId);
            });

            //notification
            builder.Entity<Notification>(entity =>
            {

                entity.HasOne(a => a.AppUser)
                .WithMany()
                .HasForeignKey(a => a.AppUserId);
            });


        }

    }
}

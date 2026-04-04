using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers.AdminEnd
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class VehicleController : ControllerBase
    {
        private readonly PathwayDBContext _db;

        public VehicleController(PathwayDBContext db)
        {
            _db = db;
        }

        //seats
        private List<Seat> GenerateVehicleSeats(int vehicleId, int capacity)
        {
            int seatsPerRow = 4;
            char rowLetter = 'A';
            int currentSeat = 0;

            var seats = new List<Seat>();

            while (currentSeat < capacity)
            {
                for (int col = 1; col <= seatsPerRow && currentSeat < capacity; col++)
                {
                    seats.Add(new Seat
                    {
                        VehicleId = vehicleId,
                        Row = rowLetter.ToString(),
                        Column = col,
                        SeatNumber = $"{rowLetter}{col}",
                        IsWindow = col == 1 || col == 4,
                        IsAisle = col == 2 || col == 3
                    });

                    currentSeat++;
                }
                rowLetter++;
            }

            return seats;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _db.Vehicle
                .Select(v => new
                {
                    v.VehicleId,
                    v.VehicleNumber,
                    v.Capacity,
                    v.Doors,
                    v.HasAC,
                    v.ImageUrl,
                    v.Status,
                    VehicleType = EF.Property<string>(v, "VehicleType")
                })
                .ToListAsync();

            return Ok(vehicles);
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicle = await _db.Vehicle
                .Include(v => v.Trips)
                .Include(v => v.Seats)  // Include seat layout
                .Include(v => v.VehicleMaintenances)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }

        [HttpPost("bus")]
        public async Task<IActionResult> CreateBus(BusCreateDto dto)
        {
            if (await _db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            var vehicle = new Bus
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                Status = dto.Status,
                HasAC = dto.HasAC,
                StandingCapacity = dto.StandingCapacity
            };

            _db.Vehicle.Add(vehicle);
            await _db.SaveChangesAsync();

            var seats = GenerateVehicleSeats(vehicle.VehicleId, vehicle.Capacity);
            await _db.Seat.AddRangeAsync(seats);
            await _db.SaveChangesAsync();

            return Ok(vehicle);
        }

     
        [HttpPost("minibus")]
        public async Task<IActionResult> CreateMiniBus(MiniBusCreateDto dto)
        {
            if (await _db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            var vehicle = new MiniBus
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                Status = dto.Status,
                HasAC = dto.HasAC,
                StandingCapacity = dto.StandingCapacity
            };

            _db.Vehicle.Add(vehicle);
            await _db.SaveChangesAsync();

            var seats = GenerateVehicleSeats(vehicle.VehicleId, vehicle.Capacity);
            await _db.Seat.AddRangeAsync(seats);
            await _db.SaveChangesAsync();

            return Ok(vehicle);
        }

        [HttpPost("car")]
        public async Task<IActionResult> CreateCar(CarCreateDto dto)
        {
            if (await _db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            var vehicle = new Car
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                Status = dto.Status,
                HasAC = dto.HasAC,
                CarCategory = dto.CarCategory
            };

            _db.Vehicle.Add(vehicle);
            await _db.SaveChangesAsync();

            return Ok(vehicle);
        }

        [HttpPost("micro")]
        public async Task<IActionResult> CreateMicro(MicroCreateDto dto)
        {
            if (await _db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            var vehicle = new Micro
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                Status = dto.Status,
                HasAC = dto.HasAC,
                MicroCategory = dto.MicroCategory
            };

            _db.Vehicle.Add(vehicle);
            await _db.SaveChangesAsync();

            return Ok(vehicle);
        }

        [HttpPut("bus/{id}")]
        public async Task<IActionResult> UpdateBus(int id, BusCreateDto dto)
        {
            var vehicle = await _db.Vehicle.OfType<Bus>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;
            vehicle.StandingCapacity = dto.StandingCapacity;

            await _db.SaveChangesAsync();
            return Ok(vehicle);
        }

        [HttpPut("minibus/{id}")]
        public async Task<IActionResult> UpdateMiniBus(int id, MiniBusCreateDto dto)
        {
            var vehicle = await _db.Vehicle.OfType<MiniBus>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;
            vehicle.StandingCapacity = dto.StandingCapacity;

            await _db.SaveChangesAsync();
            return Ok(vehicle);
        }

        [HttpPut("car/{id}")]
        public async Task<IActionResult> UpdateCar(int id, CarCreateDto dto)
        {
            var vehicle = await _db.Vehicle.OfType<Car>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;
            vehicle.CarCategory = dto.CarCategory;

            await _db.SaveChangesAsync();
            return Ok(vehicle);
        }

        [HttpPut("micro/{id}")]
        public async Task<IActionResult> UpdateMicro(int id, MicroCreateDto dto)
        {
            var vehicle = await _db.Vehicle.OfType<Micro>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;
            vehicle.MicroCategory = dto.MicroCategory;

            await _db.SaveChangesAsync();
            return Ok(vehicle);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _db.Vehicle
                .Include(v => v.Trips)
                .Include(v => v.Seats)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null) return NotFound();

            if (vehicle.Trips != null && vehicle.Trips.Any())
                return BadRequest("Cannot delete vehicle assigned to trips.");

            _db.Vehicle.Remove(vehicle);
            await _db.SaveChangesAsync();
            return Ok("Vehicle deleted successfully");
        }

        //get by type
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(string type)
        {
            var vehicles = await _db.Vehicle
                .Where(v => EF.Property<string>(v, "VehicleType") == type)
                .ToListAsync();

            return Ok(vehicles);
        }
    }
}
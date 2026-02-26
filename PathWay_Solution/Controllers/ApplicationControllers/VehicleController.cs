using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class VehicleController(PathwayDBContext db) : ControllerBase     //done but critical one..need to overview
    {
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await db.Vehicle
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
            var vehicle = await db.Vehicle
                .Include(v => v.Trips)
                .Include(v => v.VehicleMaintenances)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }

        [HttpPost("bus")]
        public async Task<IActionResult> CreateBus(BusCreateDto dto)
        {
            if (await db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            Vehicle vehicle;

            vehicle = new Bus
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                Status = dto.Status,
                HasAC = dto.HasAC,
                StandingCapacity = dto.StandingCapacity
            };

            db.Vehicle.Add(vehicle);
            await db.SaveChangesAsync();

            return Ok(vehicle);
        }
        [HttpPost("minibus")]
        public async Task<IActionResult> CreateMiniBus(MiniBusCreateDto dto)
        {
            if (await db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            Vehicle vehicle;

            vehicle = new MiniBus
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                Status = dto.Status,
                HasAC = dto.HasAC,
                StandingCapacity = dto.StandingCapacity
            };

            db.Vehicle.Add(vehicle);
            await db.SaveChangesAsync();

            return Ok(vehicle);
        }
        [HttpPost("car")]
        public async Task<IActionResult> CreateCar(CarCreateDto dto)
        {
            if (await db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            Vehicle vehicle;


            vehicle = new Car
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                HasAC = dto.HasAC,
                Status = dto.Status,
                CarCategory = dto.CarCategory
            };

            db.Vehicle.Add(vehicle);
            await db.SaveChangesAsync();

            return Ok(vehicle);
        }
        [HttpPost("micro")]
        public async Task<IActionResult> CreateBus(MicroCreateDto dto)
        {
            if (await db.Vehicle.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber))
                return BadRequest("Vehicle number already exists");

            Vehicle vehicle;

            vehicle = new Micro
            {
                VehicleNumber = dto.VehicleNumber,
                Capacity = dto.Capacity,
                Doors = dto.Doors,
                ImageUrl = dto.ImageUrl ?? "",
                Status = dto.Status,
                HasAC = dto.HasAC,
                MicroCategory = dto.MicroCategory!
            };

            db.Vehicle.Add(vehicle);
            await db.SaveChangesAsync();

            return Ok(vehicle);
        }


        [HttpPut("bus{id}")]
        public async Task<IActionResult> UpdateBus(int id, BusCreateDto dto)
        {
            var vehicle = await db.Vehicle.OfType<Bus>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;
            vehicle.StandingCapacity = dto.StandingCapacity;

            await db.SaveChangesAsync();
            return Ok(vehicle);
        }
        [HttpPut("minibus{id}")]
        public async Task<IActionResult> UpdateMiniBus(int id, MiniBusCreateDto dto)
        {
            var vehicle = await db.Vehicle.OfType<MiniBus>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;
            vehicle.StandingCapacity = dto.StandingCapacity;

            await db.SaveChangesAsync();
            return Ok(vehicle);
        }
        [HttpPut("car{id}")]
        public async Task<IActionResult> UpdateCar(int id, CarCreateDto dto)
        {
            var vehicle = await db.Vehicle.OfType<Car>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;


            await db.SaveChangesAsync();
            return Ok(vehicle);
        }
        [HttpPut("micro{id}")]
        public async Task<IActionResult> UpdateMicro(int id, MicroCreateDto dto)
        {
            var vehicle = await db.Vehicle.OfType<Micro>().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null) return NotFound();

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Capacity = dto.Capacity;
            vehicle.Doors = dto.Doors;
            vehicle.ImageUrl = dto.ImageUrl ?? "";
            vehicle.Status = dto.Status;
            vehicle.HasAC = dto.HasAC;


            await db.SaveChangesAsync();
            return Ok(vehicle);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await db.Vehicle
                .Include(v => v.Trips)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
                return NotFound();

            if (vehicle.Trips != null && vehicle.Trips.Any())
                return BadRequest("Cannot delete vehicle assigned to trips.");

            db.Vehicle.Remove(vehicle);
            await db.SaveChangesAsync();
            return Ok("Vehicle deleted successfully");
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(string type)
        {
            var vehicles = await db.Vehicle
                .Where(v => EF.Property<string>(v, "VehicleType") == type)
                .ToListAsync();

            return Ok(vehicles);
        }

    }
}

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

    public class ExpenseController(PathwayDBContext db) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateExpense(ExpenseCreateDto dto)
        {
          
            // Vehicle required for Fuel & Maintenance
            if ((dto.ExpenseType == ExpenseType.Fuel || dto.ExpenseType == ExpenseType.Maintenance)
                && dto.VehicleId == null)
            {
                return BadRequest("VehicleId is required for Fuel and Maintenance expenses.");
            }

            // Office expense must not have Vehicle or Trip
            if (dto.ExpenseType == ExpenseType.Office)
            {
                if (dto.VehicleId != null || dto.TripId != null)
                {
                    return BadRequest("Office expense cannot be linked to Vehicle or Trip.");
                }
            }

            // Optional: check Vehicle exists
            if (dto.VehicleId != null)
            {
                var vehicleExists = await db.Vehicle.AnyAsync(v => v.VehicleId == dto.VehicleId);
                if (!vehicleExists)
                    return NotFound("Vehicle not found.");
            }

            // Optional: check Trip exists
            if (dto.TripId != null)
            {
                var tripExists = await db.Trip.AnyAsync(t => t.TripId == dto.TripId);
                if (!tripExists)
                    return NotFound("Trip not found.");
            }

            var expense = new Expense
            {
                ExpenseType = dto.ExpenseType,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Description = dto.Description,
                VehicleId = dto.VehicleId,
                TripId = dto.TripId
            };

            db.Expense.Add(expense);
            await db.SaveChangesAsync();

            return Ok("Expense created successfully.");
        }


        [HttpGet]
        public async Task<IActionResult> GetExpenses()
        {
            var expenses = await db.Expense
                .Include(e => e.Vehicle)
                .Include(e => e.Trip)
                .Select(e => new ExpenseResponseDto
                {
                    ExpenseId = e.ExpenseId,
                    ExpenseType = e.ExpenseType,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate,
                    Description = e.Description,

                    VehicleId = e.VehicleId,
                    VehicleName = e.Vehicle != null ? e.Vehicle.VehicleNumber : null,

                    TripId = e.TripId,
                })
                .ToListAsync();

            return Ok(expenses);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, UpdateExpenseDto dto)
        {
            var expense = await db.Expense.FindAsync(id);

            if (expense == null)
                return NotFound("Expense not found.");

            // VALIDATION (same as Create)

            if ((dto.ExpenseType == ExpenseType.Fuel || dto.ExpenseType == ExpenseType.Maintenance)
                && dto.VehicleId == null)
            {
                return BadRequest("VehicleId is required for Fuel and Maintenance expenses.");
            }

            if (dto.ExpenseType == ExpenseType.Office)
            {
                if (dto.VehicleId != null || dto.TripId != null)
                {
                    return BadRequest("Office expense cannot be linked to Vehicle or Trip.");
                }
            }

            // Check Vehicle exists
            if (dto.VehicleId != null)
            {
                var vehicleExists = await db.Vehicle.AnyAsync(v => v.VehicleId == dto.VehicleId);
                if (!vehicleExists)
                    return NotFound("Vehicle not found.");
            }

            // Check Trip exists
            if (dto.TripId != null)
            {
                var tripExists = await db.Trip.AnyAsync(t => t.TripId == dto.TripId);
                if (!tripExists)
                    return NotFound("Trip not found.");
            }

            // Update fields
            expense.ExpenseType = dto.ExpenseType;
            expense.Amount = dto.Amount;
            expense.ExpenseDate = dto.ExpenseDate;
            expense.Description = dto.Description;
            expense.VehicleId = dto.VehicleId;
            expense.TripId = dto.TripId;

            await db.SaveChangesAsync();

            return Ok("Expense updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await db.Expense.FindAsync(id);

            if (expense == null)
                return NotFound("Expense not found.");

            db.Expense.Remove(expense);
            await db.SaveChangesAsync();

            return Ok("Expense deleted successfully.");
        }
    }
}

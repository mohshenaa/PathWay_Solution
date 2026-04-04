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

    public class SalaryController(PathwayDBContext db) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateSalary(SalaryCreateDto dto)
        {
            var exists = await db.Salary
                .AnyAsync(s => s.EmployeeId == dto.EmployeeId
                            && s.Month == dto.Month
                            && s.Year == dto.Year);

            if (exists)
                return BadRequest("Salary already exists for this month");

            var salary = new Salary
            {
                EmployeeId = dto.EmployeeId,
                Month = dto.Month,
                Year = dto.Year,
                BasicAmount = dto.BasicAmount,
                Bonus = dto.Bonus,
                Deduction = dto.Deduction,
                NetAmount = dto.BasicAmount + dto.Bonus - dto.Deduction,
                Status = "Pending"
            };

            db.Salary.Add(salary);
            await db.SaveChangesAsync();

            return Ok(salary);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSalaryDto dto)
        {
            var salary = await db.Salary.FindAsync(id);

            if (salary == null)
                return NotFound();

            if (salary.Status == "Paid")
                return BadRequest("Cannot update paid salary");

            salary.BasicAmount = dto.BasicAmount;
            salary.Bonus = dto.Bonus;
            salary.Deduction = dto.Deduction;

            // Recalculate
            salary.NetAmount = dto.BasicAmount + dto.Bonus - dto.Deduction;

            await db.SaveChangesAsync();

            return Ok(salary);
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var salary = await db.Salary.FindAsync(id);

            if (salary == null)
                return NotFound();

            if (salary.Status == "Paid")
                return BadRequest("Already paid");

            salary.Status = "Paid";
            salary.PaidDate = DateTime.UtcNow;

            //  CREATE EXPENSE
            var expense = new Expense
            {
                ExpenseType = ExpenseType.Salary,
                Amount = salary.NetAmount,
                ExpenseDate = DateTime.UtcNow,
                SalaryId = salary.SalaryId,
                Note = $"Salary paid for EmployeeId: {salary.EmployeeId} ({salary.Month}/{salary.Year})"
            };

            db.Expense.Add(expense);

            await db.SaveChangesAsync();

            return Ok("Salary paid & expense recorded");
        }
    }
}

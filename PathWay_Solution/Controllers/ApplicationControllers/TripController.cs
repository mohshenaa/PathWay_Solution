using Microsoft.AspNetCore.Mvc;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Services;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class TripController(ITripService ts) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(TripCreateDto dto)
        {
            try
            {
                var result = await ts.CreateTripAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.TripId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await ts.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await ts.GetTripByIdAsync(id);
            if (trip == null) return NotFound();
            return Ok(trip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,TripUpdateDto dto)
        {
            var result = await ts.UpdateTripAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await ts.DeleteTripAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var result = await ts.StartTripAsync(id);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await ts.CompleteTripAsync(id);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await ts.CancelTripAsync(id);
            if (!result) return BadRequest();
            return Ok();
        }
    }
}

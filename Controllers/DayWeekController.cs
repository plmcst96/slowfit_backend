using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Controllers
{
    [Route("slowFit/dayWeek")]
    [ApiController]
    public class DayWeekController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        // GET: tutti i giorni
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DayWeekRes>>> GetAll()
        {
            var days = await _slowFitContext.DayWeeks
                .Select(d => new DayWeekRes
                {
                    DayId = d.DayId,
                    DayString = d.DayString
                })
                .ToListAsync();

            if (days.Count == 0) return NoContent();
            return Ok(days);
        }

        // GET: singolo giorno per ID
        [HttpGet("{id}")]
        public async Task<ActionResult<DayWeekRes>> GetById(int id)
        {
            var day = await _slowFitContext.DayWeeks
                .Where(d => d.DayId == id)
                .Select(d => new DayWeekRes
                {
                    DayId = d.DayId,
                    DayString = d.DayString
                })
                .FirstOrDefaultAsync();

            if (day == null) return NotFound($"DayWeek with ID {id} not found.");
            return Ok(day);
        }

        // POST: crea nuovo giorno
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DayWeekRes request)
        {
            if (request == null || string.IsNullOrEmpty(request.DayString))
                return BadRequest("Invalid day data.");

            var day = new DayWeek { DayString = request.DayString };

            try
            {
                _slowFitContext.DayWeeks.Add(day);
                await _slowFitContext.SaveChangesAsync();
                request.DayId = day.DayId; // restituisce l'ID generato
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create DayWeek: {ex.Message}");
            }
        }

        // PUT: aggiorna giorno esistente
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DayWeekRes request)
        {
            var day = await _slowFitContext.DayWeeks.FirstOrDefaultAsync(d => d.DayId == id);
            if (day == null) return NotFound($"DayWeek with ID {id} not found.");

            day.DayString = request.DayString;

            try
            {
                await _slowFitContext.SaveChangesAsync();
                return Ok($"DayWeek with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update DayWeek: {ex.Message}");
            }
        }

        // DELETE: elimina giorno
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var day = await _slowFitContext.DayWeeks.FirstOrDefaultAsync(d => d.DayId == id);
            if (day == null) return NotFound($"DayWeek with ID {id} not found.");

            try
            {
                _slowFitContext.DayWeeks.Remove(day);
                await _slowFitContext.SaveChangesAsync();
                return Ok($"DayWeek with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete DayWeek: {ex.Message}");
            }
        }
    }
}

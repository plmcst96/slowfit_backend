using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/measure")]
    [ApiController]
    public class MeasureController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        // GET: api/<MeasureController>
        [HttpGet]
        public ActionResult<IEnumerable<MeasureRes>> Get()
        {
            var measureList = new List<MeasureRes>();
            try
            {

                measureList = _slowFitContext.Measures.Select(t => new MeasureRes
                {
                    MeasureId = t.MeasureId,
                    Cm = t.Cm,
                    BodyId = t.BodyId,
                    CollectPeriod = DateTime.ParseExact(t.CollectPeriod, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                }).ToList();


                if (measureList.Count == 0) NoContent();

                return Ok(measureList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        // GET api/<MeasureController>/5
        [HttpGet("{id}")]
        public ActionResult<MeasureRes> GetMeasureById(int id)
        {
            try
            {
                var measure = _slowFitContext.Measures.Where(t => t.MeasureId == id).FirstOrDefault();
                if (measure == null) return NotFound();
                return Ok(measure);
            }
            catch (Exception ex)
            {
                return BadRequest($"No measure found with {id}");
            }

        }

        [HttpGet("byDate")]
        public ActionResult<IEnumerable<MeasureRes>> GetMeasureByDate([FromQuery] DateTime date)
        {
            try
            {
                var filteredMeasure = _slowFitContext.Measures.Where(t => DateTime.ParseExact(t.CollectPeriod, "yyyy-MM-dd", CultureInfo.InvariantCulture) == date.Date).ToList();
                if (filteredMeasure.Count == 0) return NotFound($"No trainings found for this {date}.");
                return Ok(filteredMeasure);
            }
            catch (Exception ex)
            {
                return BadRequest($"No measure found in this date {date}.");
            }

        }

        [HttpGet("byUser/{userId}")]
        public ActionResult<IEnumerable<MeasureRes>> GetMeasuresByUserId(int userId)
        {
            try
            {
                var measures = _slowFitContext.Measures
                    .Where(m => m.UserId == userId)
                    .Select(m => new MeasureRes
                    {
                        MeasureId = m.MeasureId,
                        Cm = m.Cm,
                        BodyId = m.BodyId,
                        UserId = m.UserId,
                        CollectPeriod = DateTime.ParseExact(m.CollectPeriod, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    })
                    .ToList();

                if (measures.Count == 0)
                    return NotFound($"No measures found for user with ID {userId}.");

                return Ok(measures);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while retrieving the user's measures.");
            }
        }




        // POST api/<MeasureController>
        [HttpPost]
        public IActionResult CreateMeasure([FromBody] MeasureRes measure)
        {
            if (measure == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (measure.UserId <= 0 || measure.BodyId <= 0 || measure.Cm <= 0)
            {
                return BadRequest();
            }

            if (measure.CollectPeriod == default)
            {
                return BadRequest("Collect piriod must be valid date values.");
            }

            try
            {
                var mea = new Measure()
                {
                    UserId = measure.UserId,
                    BodyId = measure.BodyId,
                    Cm = measure.Cm,
                    CollectPeriod = measure.CollectPeriod.ToString("yyyy-MM-dd"),
                };
                _slowFitContext.Measures.Add(mea);
                _slowFitContext.SaveChanges();
                return Ok("Measure created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create measure");
            }
        }

        // PUT api/<MeasureController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateMeasure(int id, [FromBody] MeasureRes updateMeasure)
        {
            var measure = _slowFitContext.Measures.Where(t => t.MeasureId == id).FirstOrDefault();
            if (measure == null) return NotFound();

            measure.BodyId = updateMeasure.BodyId;
            measure.Cm = updateMeasure.Cm;
            measure.CollectPeriod = updateMeasure.CollectPeriod.ToString("yyyy-MM-dd");
          
            try
            {
                _slowFitContext.Measures.Update(measure);


                _slowFitContext.SaveChanges();

                return Ok("Measure updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update measure: {updateMeasure.MeasureId}");
            }
        }

        // DELETE api/<MeasureController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteMeasure(int id)
        {
            var measure = _slowFitContext.Measures.Where(t => t.MeasureId == id).FirstOrDefault();
            if (measure == null) return NotFound();

            try
            {
                _slowFitContext.Measures.Remove(measure);
                _slowFitContext.SaveChanges();

                return Ok($"The measure has been successfully cancelled");
            }
            catch (Exception ex)
            {
                return BadRequest($"No measure found whit {id}");
            }
        }
    }
}

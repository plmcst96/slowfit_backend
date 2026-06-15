using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/level")]
    [ApiController]
    public class LevelTrainingController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        // GET: api/<LevelTrainingController>
        [HttpGet]
        public ActionResult<IEnumerable<LevelTrainingRes>> GetAll()
        {
            var levelList = new List<LevelTrainingRes>();

            try
            {
                levelList = _slowFitContext.LevelTrainings.Select(n => new LevelTrainingRes
                {
                    LevelId = n.LevelId,
                    LevelString = n.LevelString,
                }).ToList();
                if (levelList.Count == 0) return NoContent();
                return Ok(levelList);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<LevelTrainingRes> GetById(int id)
        {
            try
            {
                var type = _slowFitContext.LevelTrainings
                    .Where(p => p.LevelId == id)
                    .Select(p => new LevelTrainingRes
                    {
                        LevelId = p.LevelId,
                        LevelString = p.LevelString,
                    })
                    .FirstOrDefault();

                if (type == null)
                {
                    return NotFound($"No level type found with ID {id}");
                }

                return Ok(type);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while retrieving the level training.");
            }
        }

        // POST api/<LevelTrainingController>
        [HttpPost]
        public IActionResult CreateLevel([FromBody] LevelTrainingRes level)
        {
            if (level == null)
            {
                return BadRequest("Data type incorrect");
            }
            if (string.IsNullOrEmpty(level.LevelString))
            {
                return BadRequest();
            }
            try
            {
                var type = new LevelTraining()
                {
                    LevelString = level.LevelString,
                };
                _slowFitContext.LevelTrainings.Add(type);
                _slowFitContext.SaveChanges();
                return Ok(type);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }

        // PUT api/<LevelTrainingController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateLevele(int id, [FromBody] LevelTrainingRes updateLevel)
        {
            if (updateLevel == null)
            {
                return BadRequest("Data type incorrect");
            }
            if (string.IsNullOrEmpty(updateLevel.LevelString))
            {
                return BadRequest();
            }
            try
            {
                var level = _slowFitContext.LevelTrainings.FirstOrDefault(x => x.LevelId == id);
                if (level == null)
                {
                    return NotFound();
                }
                level.LevelString = updateLevel.LevelString;
                _slowFitContext.SaveChanges();
                return Ok(level);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }

        // DELETE api/<LevelTrainingController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLevel(int id)
        {
            try
            {
                var level = _slowFitContext.LevelTrainings.FirstOrDefault(x => x.LevelId == id);
                if (level == null)
                {
                    return NotFound("Level not found");
                }
                _slowFitContext.LevelTrainings.Remove(level);
                _slowFitContext.SaveChanges();
                return Ok("Level training deleted successfully.");
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }
    }
}

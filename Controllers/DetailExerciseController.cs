using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/exercise")]
    [ApiController]
    public class DetailExerciseController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;
        // GET: api/<DetailExerciseController>
        [HttpGet("detail")]
        public ActionResult<IEnumerable<DetailExerciseRes>> GetAll()
        {
            var detailExercises = new List<DetailExerciseRes>();
            try
            {
                detailExercises = _slowFitContext.DetailExercises.Select(n => new DetailExerciseRes
                {
                    DetailExerciseId = n.DetailExerciseId,
                    NRipetition = n.NRipetition,
                    Pause = n.Pause,
                    Phase = n.Phase,
                    Series = n.Series,
                    ExerciseId = n.ExerciseId,
                    TrainingId = n.TrainingId
                }).ToList();
                if (detailExercises.Count == 0) return NoContent();
                return Ok(detailExercises);
            } catch (Exception ex) {
                return BadRequest($"An error occurred");
            }
        }

        // GET api/<DetailExerciseController>/5
        [HttpGet("detail/{id}")]
        public ActionResult<DetailExerciseRes> GetSingleDetail(int id)
        {
            try
            {
                var detailExercise = _slowFitContext.DetailExercises
                    .Where(n => n.DetailExerciseId == id)
                    .Select(n => new DetailExerciseRes
                    {
                        DetailExerciseId = n.DetailExerciseId,
                        NRipetition = n.NRipetition,
                        Pause = n.Pause,
                        Phase = n.Phase,
                        Series = n.Series,
                        ExerciseId = n.ExerciseId,
                        TrainingId = n.TrainingId
                    }).FirstOrDefault();
                if (detailExercise == null) return NoContent();
                return Ok(detailExercise);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("detailTraining/{trainingId}")]
        public ActionResult<IEnumerable<DetailExerciseRes>> GetSingleDetailByTraining(int trainingId)
        {
            try
            {
                var detailExercise = _slowFitContext.DetailExercises
                    .Where(n => n.TrainingId == trainingId)
                    .Select(n => new DetailExerciseRes
                    {
                        DetailExerciseId = n.DetailExerciseId,
                        NRipetition = n.NRipetition,
                        Pause = n.Pause,
                        Phase = n.Phase,
                        ExerciseId = n.ExerciseId,
                        TrainingId = n.TrainingId,
                        Series = n.Series
                    }).ToList();
                if (detailExercise == null) return NoContent();
                return Ok(detailExercise);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        // POST api/<DetailExerciseController>
        [HttpPost("detail")]
        public IActionResult CreateDetail([FromBody] DetailExerciseRes detailExercise)
        {
            if (detailExercise == null)
            {
                return BadRequest("Data type incorrect");
            }
            if (string.IsNullOrEmpty(detailExercise.Phase) ||
                detailExercise.ExerciseId <= 0 ||
                detailExercise.NRipetition <= 0 ||
                detailExercise.Pause <= 0)
            {
                return BadRequest();
            }

            try
            {
                var detail = new DetailExercise()
                {
                    Phase = detailExercise.Phase,
                    ExerciseId = detailExercise.ExerciseId,
                    NRipetition = detailExercise.NRipetition,
                    Pause = detailExercise.Pause,
                    Series = detailExercise.Series,
                };
                _slowFitContext.DetailExercises.Add(detail);
                _slowFitContext.SaveChanges();
                return Ok("Detail exercise created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create Detail exercise");
            }
        }

        // PUT api/<DetailExerciseController>/5
        [HttpPut("detail/{id}")]
        public IActionResult UpdateDetail(int id, [FromBody] DetailExerciseRes updateDetail)
        {
            if(updateDetail == null || updateDetail.DetailExerciseId != id)
            {
                return BadRequest("Data type incorrect");
            }
            var existingDetail = _slowFitContext.DetailExercises.FirstOrDefault(n => n.DetailExerciseId == id);
            if (existingDetail == null)
            {
                return NotFound();
            }

            existingDetail.NRipetition = updateDetail.NRipetition;
            existingDetail.Pause = updateDetail.Pause;
            existingDetail.Phase = updateDetail.Phase;
            existingDetail.ExerciseId = updateDetail.ExerciseId;
            existingDetail.TrainingId = updateDetail.TrainingId;
            existingDetail.Series = updateDetail.Series;
            try
            {
                _slowFitContext.DetailExercises.Update(existingDetail);
                _slowFitContext.SaveChanges();
                return Ok("Detail exercise updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Detail exercise");
            }
        }

        // DELETE api/<DetailExerciseController>/5
        [HttpDelete("detail/{id}")]
        public IActionResult DeleteDetail(int id)
        {
            var detail = _slowFitContext.DetailExercises.FirstOrDefault(n => n.DetailExerciseId == id);
            if (detail == null)
            {
                return NotFound();
            }
            try
            {
                _slowFitContext.DetailExercises.Remove(detail);
                _slowFitContext.SaveChanges();
                return Ok("Detail exercise deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete Detail exercise");
            }
        }
    }
}

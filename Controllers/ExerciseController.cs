using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/api/exercise")]
    [ApiController]
    public class ExerciseController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        // GET: api/<ExerciseController>
        [HttpGet]
        public ActionResult<IEnumerable<ExerciseRes>> Get()
        {
            var exerciseList = new List<ExerciseRes>();
            try
            {

                exerciseList = _slowFitContext.Exercises.Select(t => new ExerciseRes
                {
                    ExerciseId = t.ExerciseId,
                    Name = t.Name,
                    Description = t.Description,
                    UrlVideo = t.UrlVideo!,
                    Image = t.Image,
                    TypeTrainingId = t.TypeTrainingId,
                    LocationTrainingId = t.LocationTrainingId
                }).ToList();


                if (exerciseList.Count == 0) return NoContent();

                return Ok(exerciseList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        // GET api/<ExerciseController>/5
        [HttpGet("{id}")]
        public ActionResult<ExerciseRes> GetSingleExercise(int id)
        {
            try
            {
                var exercise = _slowFitContext.Exercises.Where(t => t.ExerciseId == id).FirstOrDefault();
                if (exercise == null) return NotFound();
                return Ok(exercise);
            }
            catch (Exception ex)
            {
                return BadRequest($"No exercise found with {id}");
            }
        }

        [HttpGet("exercise/{locationId}")]
        public async Task<ActionResult<IEnumerable<ExerciseRes>>> GetExercisesByLocationId(int locationId)
        {
            try
            {
                var exercises = await _slowFitContext.LocationTrainings
                    .Where(t => t.LocationId == locationId)
                    .SelectMany(t => _slowFitContext.Exercises.Where(e => e.LocationTrainingId == t.LocationId))
                    .Select(e => new ExerciseRes
                    {
                        ExerciseId = e.ExerciseId,
                        Name = e.Name,
                        Description = e.Description,
                        UrlVideo = e.UrlVideo!,
                        Image = e.Image,
                        TypeTrainingId = e.TypeTrainingId,
                        LocationTrainingId = e.LocationTrainingId
                    }).ToListAsync();

                if (exercises.Count == 0)
                {
                    return NotFound("No exercises found for the given location ID.");
                }

                return Ok(exercises);
            } catch(Exception ex)
            {
                return BadRequest($"No exercise found with location id {locationId}");
            }
        }

        [HttpGet("exerciseByTraining/{trainingId}")]
        public async Task<ActionResult<IEnumerable<ExerciseRes>>> GetExercisesByTypeTraining(int trainingId)
        {
            try
            {
                var exercises = await _slowFitContext.TypeTrainigs
                    .Where(t => t.TypeId == trainingId)
                    .SelectMany(t => _slowFitContext.Exercises.Where(e => e.TypeTrainingId == t.TypeId))
                    .Select(e => new ExerciseRes
                    {
                        ExerciseId = e.ExerciseId,
                        Name = e.Name,
                        Description = e.Description,
                        UrlVideo = e.UrlVideo!,
                        Image = e.Image,
                        TypeTrainingId = e.TypeTrainingId,
                        LocationTrainingId = e.LocationTrainingId
                    }).ToListAsync();

                if (exercises.Count == 0)
                {
                    return NotFound("No exercises found for the given location ID.");
                }

                return Ok(exercises);
            } catch(Exception ex)
            {
                return BadRequest($"No exercise found for type di {trainingId}");
            }
        }

        // POST api/<ExerciseController>
        [HttpPost]
        public IActionResult CreateExercise([FromBody] ExerciseRes exercise)
        {
            if (exercise == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (exercise.TypeTrainingId <= 0 ||
                string.IsNullOrEmpty(exercise.Name) ||
                string.IsNullOrEmpty(exercise.Description) ||
                string.IsNullOrEmpty(exercise.Image))
            {
                return BadRequest("Error");
            }

            try
            {
                var ex = new Exercise() {
                    Name = exercise.Name,
                    Description = exercise.Description,
                    UrlVideo = exercise.UrlVideo,
                    Image = exercise.Image,
                    TypeTrainingId = exercise.TypeTrainingId,
                    LocationTrainingId = exercise.LocationTrainingId,

                };
                _slowFitContext.Exercises.Add(ex);
                _slowFitContext.SaveChanges();
                return Ok("Exercise created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create exercise");
            }
        }

        // PUT api/<ExerciseController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateExercise(int id, [FromBody] ExerciseRes updateExercise)
        {
            var exercise = _slowFitContext.Exercises.Where(t => t.ExerciseId == id).FirstOrDefault();
            if (exercise == null) return NotFound();

            exercise.Name = updateExercise.Name;
            exercise.Description = updateExercise.Description;
            exercise.UrlVideo = updateExercise.UrlVideo;
            exercise.Image = updateExercise.Image;
            exercise.TypeTrainingId = updateExercise.TypeTrainingId;
            exercise.LocationTrainingId = updateExercise.LocationTrainingId;
            try
            {
                _slowFitContext.Exercises.Update(exercise);


                _slowFitContext.SaveChanges();

                return Ok(new { message = "Exercise updated succesfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Exercise: {updateExercise.Name}");
            }
        }

        // DELETE api/<ExerciseController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteExercise(int id)
        {
            var exercise = _slowFitContext.Exercises.Where(t => t.ExerciseId == id).FirstOrDefault();
            if (exercise == null) return NotFound();

            try
            {
                _slowFitContext.Exercises.Remove(exercise);
                _slowFitContext.SaveChanges();

                return Ok($"The exercise has been successfully cancelled");
            }
            catch (Exception ex)
            {
                return BadRequest($"No exercise found whit {id}");
            }
        }
    }
}

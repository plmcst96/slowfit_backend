using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slowfit.Auth;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/training")]
    [ApiController]
    public class TrainingController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        [HttpGet]
        public ActionResult<IEnumerable<TrainingRes>> GetTrainings()
        {
            if (!User.IsPersonalTrainer()) return Forbid();

            var trainingList = new List<TrainingRes>();
            try
            {

                trainingList = [.. _slowFitContext.Training.Select(t => new TrainingRes
                {
                    EndDate = DateTime.Now,
                    TrainingId = t.TrainingId,
                    CreationDate = DateTime.Now,
                    LevelId = t.LevelId,
                    UserId = t.TrainingId,
                    TypeId = t.TrainingId,
                    Duration = t.Duration
                })];


                if (trainingList.Count == 0) return NoContent();

                return Ok(trainingList);
            }
            catch (Exception)
            {
                return BadRequest( $"An error occurred");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<TrainingRes> GetTrainingById(int id)
        {
            try
            {
                var training = _slowFitContext.Training.Where(t => t.TrainingId == id).FirstOrDefault();
                if (training == null) return NotFound();
                if (!User.CanAccessUser(training.UserId)) return Forbid();
                return Ok(training);
            }
            catch (Exception) { 
                return BadRequest($"No training found with {id}");
            }
            
        }

        [HttpGet("byDate")]
        public ActionResult<IEnumerable<TrainingRes>> GetTrainingByDate([FromQuery] DateTime date)
        {
            if (!User.IsPersonalTrainer()) return Forbid();

            try
            {
                var filteredTrainings = _slowFitContext.Training.Where(t => t.CreationDate == date.Date).ToList();
                if (filteredTrainings.Count == 0) return NotFound($"No trainings found for this {date}.");
                return Ok(filteredTrainings);
            }
            catch (Exception)
            {
                return BadRequest($"No trainings found in this date {date}.");
            }
            
        }

        [HttpGet("byDateRange")]
        public ActionResult<IEnumerable<TrainingRes>> GetTrainingByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (!User.IsPersonalTrainer()) return Forbid();

            try
            {
                var filteredTrainings = _slowFitContext.Training
                .Where(t => t.CreationDate >= startDate.Date && t.CreationDate <= endDate.Date)
                .ToList();
                if (filteredTrainings.Count == 0) return NotFound($"No trainings found in this date range {startDate} to {endDate}.");
                return Ok(filteredTrainings);
            }
            catch (Exception) {
                return BadRequest();
            }

            
        }

        [HttpGet("byUser/{userId}")]
        public ActionResult<IEnumerable<TrainingRes>> GetTrainingByUserId(int userId)
        {
            if (!User.CanAccessUser(userId)) return Forbid();

            try
            {
                var userTrainings = _slowFitContext.Training
                    .Include(t => t.DetailExercises)
                    .Where(t => t.UserId == userId)
                    .ToList();

                if (userTrainings.Count == 0)
                    return NotFound($"No trainings found for user {userId}.");

                var trainingList = userTrainings.Select(t => new TrainingRes
                {
                    TrainingId = t.TrainingId,
                    TypeId = t.TypeId,
                    UserId = t.UserId,
                    LevelId = t.LevelId,
                    Duration = t.Duration,
                    CreationDate = t.CreationDate ?? default,
                    EndDate = t.EndDate ?? default,
                    DetailExercises = t.DetailExercises.Select(de => new DetailExerciseRes
                    {
                        ExerciseId = de.ExerciseId,
                        NRipetition = de.NRipetition,
                        Pause = de.Pause,
                        Phase = de.Phase,
                        Series = de.Series
                    }).ToList()
                }).ToList();

                return Ok(trainingList);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while fetching trainings for user.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateTrainingDet([FromBody] TrainingRes request)
        {
            if (!User.CanAccessUser(request.UserId)) return Forbid();

            try
            {
                // 1️⃣ Creazione del training
                var training = new Training
                {
                    TypeId = request.TypeId,
                    UserId = request.UserId,
                    LevelId = request.LevelId,
                    Duration = request.Duration,
                    CreationDate = request.CreationDate.Date,
                    EndDate = request.EndDate.Date,
                    DetailExercises = new List<DetailExercise>()
                };

                _slowFitContext.Training.Add(training);
                await _slowFitContext.SaveChangesAsync(); // Salva per ottenere TrainingId

                // 2️⃣ Creazione dei DetailExercises associati
                foreach (var de in request.DetailExercises!)
                {
                    var detail = new DetailExercise
                    {
                        ExerciseId = de.ExerciseId,
                        NRipetition = de.NRipetition,
                        Pause = de.Pause,
                        Phase = de.Phase,
                        Series = de.Series,
                        TrainingId = training.TrainingId // associa al training
                    };
                    training.DetailExercises.Add(detail);
                }

                await _slowFitContext.SaveChangesAsync();

                // 3️⃣ Creazione del DTO per la risposta (senza cicli)
                var trainingDto = new TrainingDTO
                {
                    TrainingId = training.TrainingId,
                    TypeId = training.TypeId,
                    UserId = training.UserId,
                    LevelId = training.LevelId ?? 1,      // se null, assegna 1
                    Duration = training.Duration ?? 45,   // se null, assegna 45
                    CreationDate = training.CreationDate,
                    EndDate = training.EndDate,
                    DetailExercises = training.DetailExercises.Select(de => new DetailExerciseDTO
                    {
                        DetailExerciseId = de.DetailExerciseId,
                        ExerciseId = de.ExerciseId,
                        NRipetition = de.NRipetition,
                        Pause = de.Pause,
                        Phase = de.Phase,
                        Series = de.Series
                    }).ToList()
                };


                // 4️⃣ Restituisci il DTO come risposta
                return Ok(trainingDto);
            }
            catch (Exception)
            {
                // Log dell'errore se vuoi
                return StatusCode(500, "Si è verificato un errore durante la creazione dell'allenamento.");
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTraining(int id, [FromBody] TrainingRes updatedTraining)
        {
            var training = _slowFitContext.Training
                .Include(t => t.DetailExercises)
                .FirstOrDefault(t => t.TrainingId == id);

            if (training == null) return NotFound();
            if (!User.CanAccessUser(training.UserId) || !User.CanAccessUser(updatedTraining.UserId)) return Forbid();

            // Aggiorno i campi principali
            training.TypeId = updatedTraining.TypeId;
            training.UserId = updatedTraining.UserId;
            training.LevelId = updatedTraining.LevelId;
            training.Duration = updatedTraining.Duration;
            training.CreationDate = DateTime.UtcNow.Date;
            training.EndDate = updatedTraining.EndDate.Date;

            try
            {
                // Rimuovo i vecchi esercizi
                _slowFitContext.DetailExercises.RemoveRange(training.DetailExercises);

                // Aggiungo i nuovi esercizi
                training.DetailExercises = updatedTraining.DetailExercises!.Select(de => new DetailExercise
                {
                    ExerciseId = de.ExerciseId,
                    NRipetition = de.NRipetition,
                    Pause = de.Pause,
                    Phase = de.Phase,
                    Series = de.Series,
                    TrainingId = training.TrainingId // molto importante
                }).ToList();

                _slowFitContext.Training.Update(training);
                await _slowFitContext.SaveChangesAsync();

                return Ok("Training updated successfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to update training: {updatedTraining.TrainingId}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTraining(int id)
        {
            var training = _slowFitContext.Training
                .Include(t => t.DetailExercises) // includi i dettagli
                .FirstOrDefault(t => t.TrainingId == id);

            if (training == null) return NotFound();
            if (!User.CanAccessUser(training.UserId)) return Forbid();

            try
            {
                // 1️⃣ Rimuovi tutti i detail exercise associati
                _slowFitContext.DetailExercises.RemoveRange(training.DetailExercises);

                // 2️⃣ Rimuovi il training
                _slowFitContext.Training.Remove(training);

                // 3️⃣ Salva le modifiche
                _slowFitContext.SaveChanges();

                return Ok("The training and all associated exercises have been successfully deleted.");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to delete training with id {id}.");
            }
        }

    }
}

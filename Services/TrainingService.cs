using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using slowfit.Auth;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public sealed class TrainingService(SlowFitContext context) : ITrainingService
{
    private readonly SlowFitContext _context = context;

    public async Task<ServiceResult<IReadOnlyList<TrainingRes>>> GetAllAsync(ClaimsPrincipal user)
    {
        if (!user.IsPersonalTrainer())
            return Forbidden<IReadOnlyList<TrainingRes>>();

        try
        {
            var query = _context.Training.AsQueryable();

            // Un PT vede solo gli allenamenti collegati ai propri clienti; il superadmin vede tutto.
            if (!user.IsSuperAdmin())
            {
                var ptId = user.GetUserId();
                query = query.Where(t => _context.Users.Any(u => u.UserId == t.UserId && u.PtId == ptId));
            }

            var trainingList = await query.Select(t => new TrainingRes
            {
                EndDate = DateTime.Now,
                TrainingId = t.TrainingId,
                CreationDate = DateTime.Now,
                LevelId = t.LevelId,
                UserId = t.TrainingId,
                TypeId = t.TrainingId,
                Duration = t.Duration
            }).ToListAsync();

            return ServiceResult<IReadOnlyList<TrainingRes>>.Ok(trainingList);
        }
        catch (Exception)
        {
            return ServiceResult<IReadOnlyList<TrainingRes>>.Error("training_fetch_failed", "Non è stato possibile caricare gli allenamenti. Riprova.");
        }
    }

    public async Task<ServiceResult<object>> GetByIdAsync(ClaimsPrincipal user, int id)
    {
        try
        {
            var training = await _context.Training.FirstOrDefaultAsync(t => t.TrainingId == id);
            if (training == null)
                return ServiceResult<object>.Ok(new { });
            if (!await CanManageClientPlanAsync(user, training.UserId))
                return ServiceResult<object>.Forbidden("forbidden", "Non hai i permessi per eseguire questa operazione.");
            return ServiceResult<object>.Ok(training);
        }
        catch (Exception)
        {
            return ServiceResult<object>.NotFound("training_not_found", "Allenamento non trovato.");
        }
    }

    public async Task<ServiceResult<object>> GetByDateAsync(ClaimsPrincipal user, DateTime date)
    {
        if (!user.IsPersonalTrainer())
            return ServiceResult<object>.Forbidden("forbidden", "Non hai i permessi per eseguire questa operazione.");

        try
        {
            var filteredTrainings = await _context.Training.Where(t => t.CreationDate == date.Date).ToListAsync();
            return ServiceResult<object>.Ok(filteredTrainings);
        }
        catch (Exception)
        {
            return ServiceResult<object>.Error("training_fetch_failed", "Non è stato possibile caricare gli allenamenti per questa data.");
        }
    }

    public async Task<ServiceResult<object>> GetByDateRangeAsync(ClaimsPrincipal user, DateTime startDate, DateTime endDate)
    {
        if (!user.IsPersonalTrainer())
            return ServiceResult<object>.Forbidden("forbidden", "Non hai i permessi per eseguire questa operazione.");

        try
        {
            var filteredTrainings = await _context.Training
                .Where(t => t.CreationDate >= startDate.Date && t.CreationDate <= endDate.Date)
                .ToListAsync();
            return ServiceResult<object>.Ok(filteredTrainings);
        }
        catch (Exception)
        {
            return ServiceResult<object>.Error("training_fetch_failed", "Non è stato possibile caricare gli allenamenti nel periodo selezionato.");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TrainingRes>>> GetByUserAsync(ClaimsPrincipal user, int userId)
    {
        if (!await CanManageClientPlanAsync(user, userId))
            return Forbidden<IReadOnlyList<TrainingRes>>();

        try
        {
            var userTrainings = await _context.Training
                .Include(t => t.DetailExercises)
                .Where(t => t.UserId == userId)
                .ToListAsync();

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

            return ServiceResult<IReadOnlyList<TrainingRes>>.Ok(trainingList);
        }
        catch (Exception)
        {
            return ServiceResult<IReadOnlyList<TrainingRes>>.Error("training_fetch_failed", "Non è stato possibile caricare gli allenamenti dell'utente.");
        }
    }

    public async Task<ServiceResult<object>> CreateAsync(ClaimsPrincipal user, TrainingRes request)
    {
        if (!await CanManageClientPlanAsync(user, request.UserId))
            return ServiceResult<object>.Forbidden("forbidden", "Non hai i permessi per eseguire questa operazione.");

        try
        {
            // Se a creare l'allenamento è un PT lo si attribuisce a lui; se è l'utente stesso, ptId resta null (auto-creato).
            var assigningPtId = user.GetRoleId() == 2 ? user.GetUserId() : null;

            // 1️⃣ Creazione del training
            var training = new Training
            {
                TypeId = request.TypeId,
                UserId = request.UserId,
                LevelId = request.LevelId,
                Duration = request.Duration,
                CreationDate = request.CreationDate.Date,
                EndDate = request.EndDate.Date,
                PtId = assigningPtId,
                DetailExercises = new List<DetailExercise>()
            };

            _context.Training.Add(training);
            await _context.SaveChangesAsync(); // Salva per ottenere TrainingId

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
                    Kg = de.Kg,
                    TrainingId = training.TrainingId // associa al training
                };
                training.DetailExercises.Add(detail);
            }

            await _context.SaveChangesAsync();

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
                    Series = de.Series,
                    Kg = de.Kg
                }).ToList()
            };

            return ServiceResult<object>.Ok(trainingDto);
        }
        catch (Exception)
        {
            return ServiceResult<object>.Error("training_create_failed", "Non è stato possibile creare l'allenamento. Riprova.");
        }
    }

    public async Task<ServiceResult<object>> UpdateAsync(ClaimsPrincipal user, int id, TrainingRes updatedTraining)
    {
        var training = await _context.Training
            .Include(t => t.DetailExercises)
            .FirstOrDefaultAsync(t => t.TrainingId == id);

        if (training == null)
            return ServiceResult<object>.NotFound("training_not_found", "Allenamento non trovato.");
        if (!await CanManageClientPlanAsync(user, training.UserId) || !await CanManageClientPlanAsync(user, updatedTraining.UserId))
            return ServiceResult<object>.Forbidden("forbidden", "Non hai i permessi per eseguire questa operazione.");

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
            _context.DetailExercises.RemoveRange(training.DetailExercises);

            // Aggiungo i nuovi esercizi
            training.DetailExercises = updatedTraining.DetailExercises!.Select(de => new DetailExercise
            {
                ExerciseId = de.ExerciseId,
                NRipetition = de.NRipetition,
                Pause = de.Pause,
                Phase = de.Phase,
                Series = de.Series,
                Kg = de.Kg,
                TrainingId = training.TrainingId // molto importante
            }).ToList();

            _context.Training.Update(training);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok("Training updated successfully");
        }
        catch (Exception)
        {
            return ServiceResult<object>.Error("training_update_failed", "Non è stato possibile aggiornare l'allenamento. Riprova.");
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(ClaimsPrincipal user, int id)
    {
        var training = await _context.Training
            .Include(t => t.DetailExercises) // includi i dettagli
            .FirstOrDefaultAsync(t => t.TrainingId == id);

        if (training == null)
            return ServiceResult<object>.NotFound("training_not_found", "Allenamento non trovato.");
        if (!await CanManageClientPlanAsync(user, training.UserId))
            return ServiceResult<object>.Forbidden("forbidden", "Non hai i permessi per eseguire questa operazione.");

        try
        {
            // 1️⃣ Rimuovi tutti i detail exercise associati
            _context.DetailExercises.RemoveRange(training.DetailExercises);

            // 2️⃣ Rimuovi il training
            _context.Training.Remove(training);

            // 3️⃣ Salva le modifiche
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok("The training and all associated exercises have been successfully deleted.");
        }
        catch (Exception)
        {
            return ServiceResult<object>.Error("training_delete_failed", "Non è stato possibile eliminare l'allenamento. Riprova.");
        }
    }

    // superadmin -> tutti; PT -> solo i propri clienti; utente -> solo se stesso.
    private async Task<bool> CanManageClientPlanAsync(ClaimsPrincipal user, int clientUserId)
    {
        if (user.IsSuperAdmin()) return true;
        var actingId = user.GetUserId();
        if (user.GetRoleId() == 2) // PersonalTrainerRoleId
            return await _context.Users.AnyAsync(u => u.UserId == clientUserId && u.PtId == actingId);
        return actingId == clientUserId;
    }

    private static ServiceResult<T> Forbidden<T>() =>
        ServiceResult<T>.Forbidden("forbidden", "Non hai i permessi per eseguire questa operazione.");
}

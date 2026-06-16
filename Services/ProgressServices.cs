using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IProgressTrainingService
{
    Task<ServiceResult<IReadOnlyList<ProgressTrainingRes>>> GetAllAsync();

    Task<ServiceResult<ProgressTrainingRes>> GetByIdAsync(int id);

    Task<ServiceResult<IReadOnlyList<ProgressTrainingRes>>> GetByUserAsync(int userId);

    Task<ServiceResult<IReadOnlyList<ProgressTrainingRes>>> GetByTrainingAsync(int trainingId);

    Task<ServiceResult<ProgressTrainingSaveResponse>> CreateAsync(ProgressTrainingRes request);

    Task<ServiceResult<ProgressTrainingRes>> UpdateAsync(int id, ProgressTrainingRes request);

    Task<ServiceResult<object>> DeleteAsync(int id);
}

public sealed class ProgressTrainingService(SlowFitContext context) : IProgressTrainingService
{
    private readonly SlowFitContext _context = context;

    public async Task<ServiceResult<IReadOnlyList<ProgressTrainingRes>>> GetAllAsync()
    {
        var progress = await _context.ProgressTrainings.AsNoTracking().ToListAsync();
        return ServiceResult<IReadOnlyList<ProgressTrainingRes>>.Ok(progress.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ProgressTrainingRes>> GetByIdAsync(int id)
    {
        var progress = await _context.ProgressTrainings.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return progress == null ? ServiceResult<ProgressTrainingRes>.Ok(default!) : ServiceResult<ProgressTrainingRes>.Ok(ToDto(progress));
    }

    public async Task<ServiceResult<IReadOnlyList<ProgressTrainingRes>>> GetByUserAsync(int userId)
    {
        var progress = await _context.ProgressTrainings.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
        return ServiceResult<IReadOnlyList<ProgressTrainingRes>>.Ok(progress.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<IReadOnlyList<ProgressTrainingRes>>> GetByTrainingAsync(int trainingId)
    {
        var progress = await _context.ProgressTrainings.AsNoTracking().Where(p => p.TrainingId == trainingId).ToListAsync();
        return ServiceResult<IReadOnlyList<ProgressTrainingRes>>.Ok(progress.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ProgressTrainingSaveResponse>> CreateAsync(ProgressTrainingRes request)
    {
        if (!IsValid(request))
        {
            return ServiceResult<ProgressTrainingSaveResponse>.BadRequest("invalid_progress_training", "Invalid progress training data.");
        }

        var entity = new ProgressTraining
        {
            TrainingId = request.TrainingId,
            ProgressValue = request.ProgressValue,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
            DateOfProgress = request.DateOfProgress.Date,
            AvarageKg = request.AvarageKg,
            UserId = request.UserId,
            Duration = request.Duration
        };

        _context.ProgressTrainings.Add(entity);
        await _context.SaveChangesAsync();

        return ServiceResult<ProgressTrainingSaveResponse>.Created(new ProgressTrainingSaveResponse { Progress = ToDto(entity) });
    }

    public async Task<ServiceResult<ProgressTrainingRes>> UpdateAsync(int id, ProgressTrainingRes request)
    {
        if (!IsValid(request) || request.Id != id)
        {
            return ServiceResult<ProgressTrainingRes>.BadRequest("invalid_progress_training", "Invalid data or ID does not match.");
        }

        var entity = await _context.ProgressTrainings.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
        {
            return ServiceResult<ProgressTrainingRes>.NotFound("progress_training_not_found", "Progress training not found.");
        }

        entity.TrainingId = request.TrainingId;
        entity.ProgressValue = request.ProgressValue;
        entity.CreatedAt = request.CreatedAt ?? DateTime.UtcNow;
        entity.DateOfProgress = request.DateOfProgress.Date;
        entity.AvarageKg = request.AvarageKg;
        entity.UserId = request.UserId;
        entity.Duration = request.Duration;

        await _context.SaveChangesAsync();
        return ServiceResult<ProgressTrainingRes>.Ok(ToDto(entity));
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var entity = await _context.ProgressTrainings.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
        {
            return ServiceResult<object>.NotFound("progress_training_not_found", "Progress training not found.");
        }

        _context.ProgressTrainings.Remove(entity);
        await _context.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    private static bool IsValid(ProgressTrainingRes request) => request != null && request.TrainingId > 0 && request.UserId > 0 && request.ProgressValue is >= 0 and <= 100 && request.DateOfProgress != default;

    private static ProgressTrainingRes ToDto(ProgressTraining entity) => new()
    {
        Id = entity.Id,
        TrainingId = entity.TrainingId,
        ProgressValue = entity.ProgressValue,
        CreatedAt = entity.CreatedAt,
        DateOfProgress = entity.DateOfProgress,
        AvarageKg = entity.AvarageKg,
        UserId = entity.UserId,
        Duration = entity.Duration ?? 0
    };
}

public interface IProgressNutritionService
{
    Task<ServiceResult<IReadOnlyList<ProgressNutritionRes>>> GetAllAsync();

    Task<ServiceResult<ProgressNutritionRes>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyList<ProgressNutritionRes>>> GetByUserAsync(int userId);
    Task<ServiceResult<IReadOnlyList<ProgressNutritionRes>>> GetByNutritionAsync(int nutritionId);
    Task<ServiceResult<ProgressNutritionSaveResponse>> CreateAsync(ProgressNutritionRes request);
    Task<ServiceResult<ProgressNutritionRes>> UpdateAsync(int id, ProgressNutritionRes request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}

public sealed class ProgressNutritionService(SlowFitContext context) : IProgressNutritionService
{
    private readonly SlowFitContext _context = context;

    public async Task<ServiceResult<IReadOnlyList<ProgressNutritionRes>>> GetAllAsync()
    {
        var progress = await _context.ProgressNutritions.AsNoTracking().ToListAsync();
        return ServiceResult<IReadOnlyList<ProgressNutritionRes>>.Ok(progress.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ProgressNutritionRes>> GetByIdAsync(int id)
    {
        var progress = await _context.ProgressNutritions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return progress == null ? ServiceResult<ProgressNutritionRes>.Ok(default!) : ServiceResult<ProgressNutritionRes>.Ok(ToDto(progress));
    }

    public async Task<ServiceResult<IReadOnlyList<ProgressNutritionRes>>> GetByUserAsync(int userId)
    {
        var progress = await _context.ProgressNutritions.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
        return ServiceResult<IReadOnlyList<ProgressNutritionRes>>.Ok(progress.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<IReadOnlyList<ProgressNutritionRes>>> GetByNutritionAsync(int nutritionId)
    {
        var progress = await _context.ProgressNutritions.AsNoTracking().Where(p => p.NutritionId == nutritionId).ToListAsync();
        return ServiceResult<IReadOnlyList<ProgressNutritionRes>>.Ok(progress.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ProgressNutritionSaveResponse>> CreateAsync(ProgressNutritionRes request)
    {
        if (!IsValid(request))
        {
            return ServiceResult<ProgressNutritionSaveResponse>.BadRequest("invalid_progress_nutrition", "Invalid progress nutrition data.");
        }

        var entity = new ProgressNutrition
        {
            NutritionId = request.NutritionId,
            ProgressValue = request.ProgressValue,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
            DateOfProgress = request.DateOfProgress.Date,
            AvarageKcal = request.AvarageKcal,
            UserId = request.UserId
        };

        _context.ProgressNutritions.Add(entity);
        await _context.SaveChangesAsync();

        return ServiceResult<ProgressNutritionSaveResponse>.Created(new ProgressNutritionSaveResponse { Progress = ToDto(entity) });
    }

    public async Task<ServiceResult<ProgressNutritionRes>> UpdateAsync(int id, ProgressNutritionRes request)
    {
        if (!IsValid(request) || request.Id != id)
        {
            return ServiceResult<ProgressNutritionRes>.BadRequest("invalid_progress_nutrition", "Invalid data or ID does not match.");
        }

        var entity = await _context.ProgressNutritions.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
        {
            return ServiceResult<ProgressNutritionRes>.NotFound("progress_nutrition_not_found", "Progress nutrition not found.");
        }

        entity.NutritionId = request.NutritionId;
        entity.ProgressValue = request.ProgressValue;
        entity.CreatedAt = request.CreatedAt ?? DateTime.UtcNow;
        entity.DateOfProgress = request.DateOfProgress.Date;
        entity.AvarageKcal = request.AvarageKcal;
        entity.UserId = request.UserId;

        await _context.SaveChangesAsync();
        return ServiceResult<ProgressNutritionRes>.Ok(ToDto(entity));
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var entity = await _context.ProgressNutritions.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
        {
            return ServiceResult<object>.NotFound("progress_nutrition_not_found", "Progress nutrition not found.");
        }

        _context.ProgressNutritions.Remove(entity);
        await _context.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    private static bool IsValid(ProgressNutritionRes request) => request != null && request.NutritionId > 0 && request.UserId > 0 && request.ProgressValue is >= 0 and <= 100 && request.DateOfProgress != default;

    private static ProgressNutritionRes ToDto(ProgressNutrition entity) => new()
    {
        Id = entity.Id,
        NutritionId = entity.NutritionId,
        ProgressValue = entity.ProgressValue,
        CreatedAt = entity.CreatedAt,
        DateOfProgress = entity.DateOfProgress,
        AvarageKcal = entity.AvarageKcal,
        UserId = entity.UserId
    };
}

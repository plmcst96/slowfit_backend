using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public sealed class ExerciseService(SlowFitContext context) : CrudServiceBase<Exercise, ExerciseRes>(context), IExerciseService
{
    protected override DbSet<Exercise> Set => ((SlowFitContext)DbContext).Exercises;
    protected override string EntityCode => "exercise";
    protected override string EntityName => "Exercise";
    protected override int GetId(Exercise entity) => entity.ExerciseId;
    protected override int GetDtoId(ExerciseRes dto) => dto.ExerciseId;
    protected override ExerciseRes ToDto(Exercise entity) => new()
    {
        ExerciseId = entity.ExerciseId,
        Name = entity.Name,
        Description = entity.Description,
        UrlVideo = entity.UrlVideo!,
        Image = entity.Image,
        TypeTrainingId = entity.TypeTrainingId,
        LocationTrainingId = entity.LocationTrainingId
    };
    protected override Exercise CreateEntity(ExerciseRes dto) => new()
    {
        Name = dto.Name.Trim(),
        Description = dto.Description.Trim(),
        UrlVideo = dto.UrlVideo,
        Image = dto.Image,
        TypeTrainingId = dto.TypeTrainingId,
        LocationTrainingId = dto.LocationTrainingId
    };
    protected override void UpdateEntity(Exercise entity, ExerciseRes dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description.Trim();
        entity.UrlVideo = dto.UrlVideo;
        entity.Image = dto.Image;
        entity.TypeTrainingId = dto.TypeTrainingId;
        entity.LocationTrainingId = dto.LocationTrainingId;
    }
    protected override bool IsValid(ExerciseRes dto) => dto != null &&
        dto.TypeTrainingId > 0 &&
        dto.LocationTrainingId > 0 &&
        !string.IsNullOrWhiteSpace(dto.Name) &&
        !string.IsNullOrWhiteSpace(dto.Description);

    public async Task<ServiceResult<IReadOnlyList<ExerciseRes>>> GetByLocationAsync(int locationId)
    {
        var exercises = await Set.AsNoTracking()
            .Where(e => e.LocationTrainingId == locationId)
            .Select(e => ToDto(e))
            .ToListAsync();

        return exercises.Count == 0
            ? ServiceResult<IReadOnlyList<ExerciseRes>>.NotFound("exercise_not_found", "No exercises found for the given location.")
            : ServiceResult<IReadOnlyList<ExerciseRes>>.Ok(exercises);
    }

    public async Task<ServiceResult<IReadOnlyList<ExerciseRes>>> GetByTypeTrainingAsync(int trainingTypeId)
    {
        var exercises = await Set.AsNoTracking()
            .Where(e => e.TypeTrainingId == trainingTypeId)
            .Select(e => ToDto(e))
            .ToListAsync();

        return exercises.Count == 0
            ? ServiceResult<IReadOnlyList<ExerciseRes>>.NotFound("exercise_not_found", "No exercises found for the given training type.")
            : ServiceResult<IReadOnlyList<ExerciseRes>>.Ok(exercises);
    }
}

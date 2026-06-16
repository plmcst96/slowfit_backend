using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public interface IMeasureService : ICrudService<MeasureRes>
{
    Task<ServiceResult<IReadOnlyList<MeasureRes>>> GetByDateAsync(DateTime date);
    Task<ServiceResult<IReadOnlyList<MeasureRes>>> GetByUserAsync(int userId);
}

public sealed class MeasureService(SlowFitContext context) : CrudServiceBase<Measure, MeasureRes>(context), IMeasureService
{
    protected override DbSet<Measure> Set => ((SlowFitContext)DbContext).Measures;
    protected override string EntityCode => "measure";
    protected override string EntityName => "Measure";
    protected override int GetId(Measure entity) => entity.MeasureId;
    protected override int GetDtoId(MeasureRes dto) => dto.MeasureId;
    protected override MeasureRes ToDto(Measure entity) => new() { MeasureId = entity.MeasureId, BodyId = entity.BodyId, Cm = entity.Cm, CollectPeriod = entity.CollectPeriod, UserId = entity.UserId };
    protected override Measure CreateEntity(MeasureRes dto) => new() { BodyId = dto.BodyId, Cm = dto.Cm, CollectPeriod = dto.CollectPeriod.Date, UserId = dto.UserId };
    protected override void UpdateEntity(Measure entity, MeasureRes dto)
    {
        entity.BodyId = dto.BodyId;
        entity.Cm = dto.Cm;
        entity.CollectPeriod = dto.CollectPeriod.Date;
        entity.UserId = dto.UserId;
    }
    protected override bool IsValid(MeasureRes dto) => dto != null && dto.UserId > 0 && dto.BodyId > 0 && dto.Cm > 0 && dto.CollectPeriod != default;

    public async Task<ServiceResult<IReadOnlyList<MeasureRes>>> GetByDateAsync(DateTime date)
    {
        var entities = await Set.AsNoTracking().Where(m => m.CollectPeriod == date.Date).ToListAsync();
        var measures = entities.Select(ToDto).ToList();
        return ServiceResult<IReadOnlyList<MeasureRes>>.Ok(measures);
    }

    public async Task<ServiceResult<IReadOnlyList<MeasureRes>>> GetByUserAsync(int userId)
    {
        var entities = await Set.AsNoTracking().Where(m => m.UserId == userId).ToListAsync();
        var measures = entities.Select(ToDto).ToList();
        return ServiceResult<IReadOnlyList<MeasureRes>>.Ok(measures);
    }
}

public interface IDetailExerciseService : ICrudService<DetailExerciseRes>
{
    Task<ServiceResult<IReadOnlyList<DetailExerciseRes>>> GetByTrainingAsync(int trainingId);
}

public sealed class DetailExerciseService(SlowFitContext context) : CrudServiceBase<DetailExercise, DetailExerciseRes>(context), IDetailExerciseService
{
    protected override DbSet<DetailExercise> Set => ((SlowFitContext)DbContext).DetailExercises;
    protected override string EntityCode => "detail_exercise";
    protected override string EntityName => "Detail exercise";
    protected override int GetId(DetailExercise entity) => entity.DetailExerciseId;
    protected override int GetDtoId(DetailExerciseRes dto) => dto.DetailExerciseId;
    protected override DetailExerciseRes ToDto(DetailExercise entity) => new() { DetailExerciseId = entity.DetailExerciseId, NRipetition = entity.NRipetition, Pause = entity.Pause, Phase = entity.Phase, Series = entity.Series, Kg = entity.Kg, ExerciseId = entity.ExerciseId, TrainingId = entity.TrainingId };
    protected override DetailExercise CreateEntity(DetailExerciseRes dto) => new() { NRipetition = dto.NRipetition, Pause = dto.Pause, Phase = dto.Phase, Series = dto.Series, Kg = dto.Kg, ExerciseId = dto.ExerciseId, TrainingId = dto.TrainingId };
    protected override void UpdateEntity(DetailExercise entity, DetailExerciseRes dto)
    {
        entity.NRipetition = dto.NRipetition;
        entity.Pause = dto.Pause;
        entity.Phase = dto.Phase;
        entity.Series = dto.Series;
        entity.Kg = dto.Kg;
        entity.ExerciseId = dto.ExerciseId;
        entity.TrainingId = dto.TrainingId;
    }
    protected override bool IsValid(DetailExerciseRes dto) => dto != null && dto.ExerciseId > 0 && !string.IsNullOrWhiteSpace(dto.Phase);

    public async Task<ServiceResult<IReadOnlyList<DetailExerciseRes>>> GetByTrainingAsync(int trainingId)
    {
        var entities = await Set.AsNoTracking().Where(d => d.TrainingId == trainingId).ToListAsync();
        var details = entities.Select(ToDto).ToList();
        return ServiceResult<IReadOnlyList<DetailExerciseRes>>.Ok(details);
    }
}

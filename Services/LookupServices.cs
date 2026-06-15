using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public sealed class TypeNutritionService(SlowFitContext context) : CrudServiceBase<TypeNutrition, TypeNutritionRes>(context)
{
    protected override DbSet<TypeNutrition> Set => ((SlowFitContext)DbContext).TypeNutritions;
    protected override string EntityCode => "type_nutrition";
    protected override string EntityName => "Type nutrition";
    protected override int GetId(TypeNutrition entity) => entity.TypeNutritionId;
    protected override int GetDtoId(TypeNutritionRes dto) => dto.TypeNutritionId;
    protected override TypeNutritionRes ToDto(TypeNutrition entity) => new() { TypeNutritionId = entity.TypeNutritionId, TypeNutritionName = entity.TypeNutritionName };
    protected override TypeNutrition CreateEntity(TypeNutritionRes dto) => new() { TypeNutritionName = dto.TypeNutritionName.Trim() };
    protected override void UpdateEntity(TypeNutrition entity, TypeNutritionRes dto) => entity.TypeNutritionName = dto.TypeNutritionName.Trim();
    protected override bool IsValid(TypeNutritionRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.TypeNutritionName);
}

public sealed class CategoryOfDayService(SlowFitContext context) : CrudServiceBase<CategoryOfDay, CategoryOfDayRes>(context)
{
    protected override DbSet<CategoryOfDay> Set => ((SlowFitContext)DbContext).CategoryOfDays;
    protected override string EntityCode => "category_of_day";
    protected override string EntityName => "Category of day";
    protected override int GetId(CategoryOfDay entity) => entity.CategoryId;
    protected override int GetDtoId(CategoryOfDayRes dto) => dto.CategoryId;
    protected override CategoryOfDayRes ToDto(CategoryOfDay entity) => new() { CategoryId = entity.CategoryId, MomentOfDay = entity.MomentOfDay };
    protected override CategoryOfDay CreateEntity(CategoryOfDayRes dto) => new() { MomentOfDay = dto.MomentOfDay.Trim() };
    protected override void UpdateEntity(CategoryOfDay entity, CategoryOfDayRes dto) => entity.MomentOfDay = dto.MomentOfDay.Trim();
    protected override bool IsValid(CategoryOfDayRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.MomentOfDay);
}

public sealed class DayWeekService(SlowFitContext context) : CrudServiceBase<DayWeek, DayWeekRes>(context)
{
    protected override DbSet<DayWeek> Set => ((SlowFitContext)DbContext).DayWeeks;
    protected override string EntityCode => "day_week";
    protected override string EntityName => "Day week";
    protected override int GetId(DayWeek entity) => entity.DayId;
    protected override int GetDtoId(DayWeekRes dto) => dto.DayId;
    protected override DayWeekRes ToDto(DayWeek entity) => new() { DayId = entity.DayId, DayString = entity.DayString };
    protected override DayWeek CreateEntity(DayWeekRes dto) => new() { DayString = dto.DayString.Trim() };
    protected override void UpdateEntity(DayWeek entity, DayWeekRes dto) => entity.DayString = dto.DayString.Trim();
    protected override bool IsValid(DayWeekRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.DayString);
}

public sealed class BodyPartService(SlowFitContext context) : CrudServiceBase<BodyPart, BodyPartRes>(context)
{
    protected override DbSet<BodyPart> Set => ((SlowFitContext)DbContext).BodyParts;
    protected override string EntityCode => "body_part";
    protected override string EntityName => "Body part";
    protected override int GetId(BodyPart entity) => entity.BodyPartId;
    protected override int GetDtoId(BodyPartRes dto) => dto.BodyPartId;
    protected override BodyPartRes ToDto(BodyPart entity) => new() { BodyPartId = entity.BodyPartId, BodyPartName = entity.BodyPartName };
    protected override BodyPart CreateEntity(BodyPartRes dto) => new() { BodyPartName = dto.BodyPartName.Trim() };
    protected override void UpdateEntity(BodyPart entity, BodyPartRes dto) => entity.BodyPartName = dto.BodyPartName.Trim();
    protected override bool IsValid(BodyPartRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.BodyPartName);
}

public sealed class LevelTrainingService(SlowFitContext context) : CrudServiceBase<LevelTraining, LevelTrainingRes>(context)
{
    protected override DbSet<LevelTraining> Set => ((SlowFitContext)DbContext).LevelTrainings;
    protected override string EntityCode => "level_training";
    protected override string EntityName => "Level training";
    protected override int GetId(LevelTraining entity) => entity.LevelId;
    protected override int GetDtoId(LevelTrainingRes dto) => dto.LevelId;
    protected override LevelTrainingRes ToDto(LevelTraining entity) => new() { LevelId = entity.LevelId, LevelString = entity.LevelString };
    protected override LevelTraining CreateEntity(LevelTrainingRes dto) => new() { LevelString = dto.LevelString.Trim() };
    protected override void UpdateEntity(LevelTraining entity, LevelTrainingRes dto) => entity.LevelString = dto.LevelString.Trim();
    protected override bool IsValid(LevelTrainingRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.LevelString);
}

public sealed class LocationTrainingService(SlowFitContext context) : CrudServiceBase<LocationTraining, LocationTrainingRes>(context)
{
    protected override DbSet<LocationTraining> Set => ((SlowFitContext)DbContext).LocationTrainings;
    protected override string EntityCode => "location_training";
    protected override string EntityName => "Location training";
    protected override int GetId(LocationTraining entity) => entity.LocationId;
    protected override int GetDtoId(LocationTrainingRes dto) => dto.LocationId;
    protected override LocationTrainingRes ToDto(LocationTraining entity) => new() { LocationId = entity.LocationId, LocationString = entity.LocationString };
    protected override LocationTraining CreateEntity(LocationTrainingRes dto) => new() { LocationString = dto.LocationString.Trim() };
    protected override void UpdateEntity(LocationTraining entity, LocationTrainingRes dto) => entity.LocationString = dto.LocationString.Trim();
    protected override bool IsValid(LocationTrainingRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.LocationString);
}

public sealed class InputTypeService(SlowFitContext context) : CrudServiceBase<InputType, InputTypeRes>(context)
{
    protected override DbSet<InputType> Set => ((SlowFitContext)DbContext).InputTypes;
    protected override string EntityCode => "input_type";
    protected override string EntityName => "Input type";
    protected override int GetId(InputType entity) => entity.InputTypeId;
    protected override int GetDtoId(InputTypeRes dto) => dto.InputTypeId;
    protected override InputTypeRes ToDto(InputType entity) => new() { InputTypeId = entity.InputTypeId, InputTypeName = entity.InputTypeName };
    protected override InputType CreateEntity(InputTypeRes dto) => new() { InputTypeName = dto.InputTypeName.Trim() };
    protected override void UpdateEntity(InputType entity, InputTypeRes dto) => entity.InputTypeName = dto.InputTypeName.Trim();
    protected override bool IsValid(InputTypeRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.InputTypeName);
}

public sealed class PaymentTypeService(SlowFitContext context) : CrudServiceBase<PaymentType, PaymentTypeRes>(context)
{
    protected override DbSet<PaymentType> Set => ((SlowFitContext)DbContext).PaymentTypes;
    protected override string EntityCode => "payment_type";
    protected override string EntityName => "Payment type";
    protected override int GetId(PaymentType entity) => entity.PaymentTypeId;
    protected override int GetDtoId(PaymentTypeRes dto) => dto.PaymentTypeId;
    protected override PaymentTypeRes ToDto(PaymentType entity) => new() { PaymentTypeId = entity.PaymentTypeId, PaymentTypeName = entity.PaymentTypeName };
    protected override PaymentType CreateEntity(PaymentTypeRes dto) => new() { PaymentTypeName = dto.PaymentTypeName.Trim() };
    protected override void UpdateEntity(PaymentType entity, PaymentTypeRes dto) => entity.PaymentTypeName = dto.PaymentTypeName.Trim();
    protected override bool IsValid(PaymentTypeRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.PaymentTypeName);
}

public sealed class TypePlanService(SlowFitContext context) : CrudServiceBase<TypePlan, TypePlanRes>(context)
{
    protected override DbSet<TypePlan> Set => ((SlowFitContext)DbContext).TypePlans;
    protected override string EntityCode => "type_plan";
    protected override string EntityName => "Type plan";
    protected override int GetId(TypePlan entity) => entity.TypePlaneId;
    protected override int GetDtoId(TypePlanRes dto) => dto.TypePlaneId;
    protected override TypePlanRes ToDto(TypePlan entity) => new() { TypePlaneId = entity.TypePlaneId, TypePlaneName = entity.TypePlaneName };
    protected override TypePlan CreateEntity(TypePlanRes dto) => new() { TypePlaneName = dto.TypePlaneName.Trim() };
    protected override void UpdateEntity(TypePlan entity, TypePlanRes dto) => entity.TypePlaneName = dto.TypePlaneName.Trim();
    protected override bool IsValid(TypePlanRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.TypePlaneName);
}

public sealed class TypeTrainingService(SlowFitContext context) : CrudServiceBase<TypeTrainig, TypeTrainingRes>(context)
{
    protected override DbSet<TypeTrainig> Set => ((SlowFitContext)DbContext).TypeTrainigs;
    protected override string EntityCode => "type_training";
    protected override string EntityName => "Type training";
    protected override int GetId(TypeTrainig entity) => entity.TypeId;
    protected override int GetDtoId(TypeTrainingRes dto) => dto.TypeId;
    protected override TypeTrainingRes ToDto(TypeTrainig entity) => new() { TypeId = entity.TypeId, TypeName = entity.TypeName };
    protected override TypeTrainig CreateEntity(TypeTrainingRes dto) => new() { TypeName = dto.TypeName.Trim() };
    protected override void UpdateEntity(TypeTrainig entity, TypeTrainingRes dto) => entity.TypeName = dto.TypeName.Trim();
    protected override bool IsValid(TypeTrainingRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.TypeName);
}

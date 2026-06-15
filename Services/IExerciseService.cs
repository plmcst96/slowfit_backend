using slowfit.DTORequest;

namespace slowfit.Services;

public interface IExerciseService : ICrudService<ExerciseRes>
{
    Task<ServiceResult<IReadOnlyList<ExerciseRes>>> GetByLocationAsync(int locationId);

    Task<ServiceResult<IReadOnlyList<ExerciseRes>>> GetByTypeTrainingAsync(int trainingTypeId);
}

using slowfit.DTORequest;

namespace slowfit.Services;

public interface IIngredientService : ICrudService<IngredientRes>
{
    Task<ServiceResult<IReadOnlyList<IngredientRes>>> SearchAsync(string name);
}

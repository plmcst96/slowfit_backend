using slowfit.DTORequest;

namespace slowfit.Services;

public interface INutritionService
{
    Task<ServiceResult<IReadOnlyList<NutritionRes>>> GetAllAsync();
    Task<ServiceResult<NutritionRes>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyList<NutritionRes>>> GetByUserAsync(int userId);
    Task<ServiceResult<object>> CreateAsync(NutritionResPost request);
    Task<ServiceResult<object>> UpdateAsync(int id, NutritionResPost request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}

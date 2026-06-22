using System.Security.Claims;
using slowfit.DTORequest;

namespace slowfit.Services;

public interface INutritionService
{
    Task<ServiceResult<IReadOnlyList<NutritionRes>>> GetAllAsync(ClaimsPrincipal user);
    Task<ServiceResult<NutritionRes>> GetByIdAsync(ClaimsPrincipal user, int id);
    Task<ServiceResult<IReadOnlyList<NutritionRes>>> GetByUserAsync(ClaimsPrincipal user, int userId);
    Task<ServiceResult<object>> CreateAsync(ClaimsPrincipal user, NutritionResPost request);
    Task<ServiceResult<object>> UpdateAsync(ClaimsPrincipal user, int id, NutritionResPost request);
    Task<ServiceResult<object>> DeleteAsync(ClaimsPrincipal user, int id);
}

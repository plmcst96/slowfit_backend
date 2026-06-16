using System.Security.Claims;
using slowfit.DTORequest;

namespace slowfit.Services;

public interface ITrainingService
{
    Task<ServiceResult<IReadOnlyList<TrainingRes>>> GetAllAsync(ClaimsPrincipal user);
    Task<ServiceResult<object>> GetByIdAsync(ClaimsPrincipal user, int id);
    Task<ServiceResult<object>> GetByDateAsync(ClaimsPrincipal user, DateTime date);
    Task<ServiceResult<object>> GetByDateRangeAsync(ClaimsPrincipal user, DateTime startDate, DateTime endDate);
    Task<ServiceResult<IReadOnlyList<TrainingRes>>> GetByUserAsync(ClaimsPrincipal user, int userId);
    Task<ServiceResult<object>> CreateAsync(ClaimsPrincipal user, TrainingRes request);
    Task<ServiceResult<object>> UpdateAsync(ClaimsPrincipal user, int id, TrainingRes request);
    Task<ServiceResult<object>> DeleteAsync(ClaimsPrincipal user, int id);
}

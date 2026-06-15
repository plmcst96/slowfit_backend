using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IUserService
{
    Task<ServiceResult<IReadOnlyList<UserRes>>> GetUsersByRoleAsync(int roleId);
    Task<ServiceResult<IReadOnlyList<UserRes>>> GetUsersByPtIdAsync(int ptId);
    Task<ServiceResult<IReadOnlyList<UserProfile>>> GetAllUsersAsync();
    Task<ServiceResult<UserRes>> GetProfileAsync(int userId);
    Task<ServiceResult<UserProfile>> GetProfileByEmailAsync(string email);
    Task<ServiceResult<object>> CreateProfileAsync(int userId, AddProfile request);
    Task<ServiceResult<object>> UpdateUserAsync(int userId, UserProfile request);
    Task<ServiceResult<object>> DeleteAsync(int userId);
}

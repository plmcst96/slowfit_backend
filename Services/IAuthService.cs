using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IAuthService
{
    Task<ServiceResult<UserLoginResponse>> LoginAsync(UserLogin request);
    Task<ServiceResult<UserLoginResponse>> RefreshAsync(int userId);
    Task<ServiceResult<UserMeResponse>> GetMeAsync(int userId);
}

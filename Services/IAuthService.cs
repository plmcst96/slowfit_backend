using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IAuthService
{
    Task<ServiceResult<UserLoginResponse>> LoginAsync(UserLogin request);
    Task<ServiceResult<UserLoginResponse>> RefreshAsync(RefreshTokenRequest request);
    Task<ServiceResult<object>> LogoutAsync(RefreshTokenRequest request);
    Task<ServiceResult<UserMeResponse>> GetMeAsync(int userId, int? roleId);
}

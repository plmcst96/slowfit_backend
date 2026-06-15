using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IUserRegistrationService
{
    Task<ServiceResult<UserRegisterResponse>> RegisterAsync(UserRegister request);
}

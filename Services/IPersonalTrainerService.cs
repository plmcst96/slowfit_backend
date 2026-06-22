using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IPersonalTrainerService
{
    Task<ServiceResult<IReadOnlyList<PersonalTrainerResponse>>> GetAllAsync();
    Task<ServiceResult<PersonalTrainerResponse>> GetByIdAsync(int ptId);
    Task<ServiceResult<PersonalTrainerResponse>> CreateAsync(PersonalTrainerReq request);
    Task<ServiceResult<object>> UpdateAsync(int ptId, PersonalTrainerReq request);
    Task<ServiceResult<object>> SetPasswordAsync(int ptId, SetPasswordRequest request);
    Task<ServiceResult<object>> ActivateAsync(ActivateAccountRequest request);
    Task<ServiceResult<object>> ResendActivationAsync(int ptId);
    Task<ServiceResult<object>> DeleteAsync(int ptId);
}

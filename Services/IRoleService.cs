using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IRoleService
{
    Task<ServiceResult<IReadOnlyList<RoleDto>>> GetAllAsync();

    Task<ServiceResult<RoleDto>> CreateAsync(RoleUpsertRequest request);

    Task<ServiceResult<RoleDto>> UpdateAsync(int roleId, RoleUpsertRequest request);

    Task<ServiceResult<object>> DeleteAsync(int roleId);
}

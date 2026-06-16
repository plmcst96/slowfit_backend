using Microsoft.EntityFrameworkCore;
using slowfit.DTORequest;
using slowfit.DTOResponse;
using slowfit.DBModels;

namespace slowfit.Services;

public sealed class RoleService(SlowFitContext slowFitContext) : IRoleService
{
    private readonly SlowFitContext _slowFitContext = slowFitContext;

    public async Task<ServiceResult<IReadOnlyList<RoleDto>>> GetAllAsync()
    {
        var roles = await _slowFitContext.RoleUsers
            .OrderBy(r => r.RoleId)
            .Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName
            })
            .ToListAsync();

        return ServiceResult<IReadOnlyList<RoleDto>>.Ok(roles);
    }

    public async Task<ServiceResult<RoleDto>> CreateAsync(RoleUpsertRequest request)
    {
        var validationError = Validate(request);
        if (validationError != null) return validationError;

        var normalizedRoleName = request.RoleName.Trim();
        var exists = await _slowFitContext.RoleUsers.AnyAsync(r => r.RoleName == normalizedRoleName);
        if (exists)
        {
            return ServiceResult<RoleDto>.Conflict("role_conflict", "Questo ruolo esiste già.");
        }

        var role = new RoleUser { RoleName = normalizedRoleName };
        _slowFitContext.RoleUsers.Add(role);
        await _slowFitContext.SaveChangesAsync();

        return ServiceResult<RoleDto>.Created(ToDto(role));
    }

    public async Task<ServiceResult<RoleDto>> UpdateAsync(int roleId, RoleUpsertRequest request)
    {
        var validationError = Validate(request);
        if (validationError != null) return validationError;

        var role = await _slowFitContext.RoleUsers.FirstOrDefaultAsync(r => r.RoleId == roleId);
        if (role == null)
        {
            return ServiceResult<RoleDto>.NotFound("role_not_found", "Ruolo non trovato.");
        }

        var normalizedRoleName = request.RoleName.Trim();
        var exists = await _slowFitContext.RoleUsers.AnyAsync(r => r.RoleId != roleId && r.RoleName == normalizedRoleName);
        if (exists)
        {
            return ServiceResult<RoleDto>.Conflict("role_conflict", "Questo ruolo esiste già.");
        }

        role.RoleName = normalizedRoleName;
        await _slowFitContext.SaveChangesAsync();

        return ServiceResult<RoleDto>.Ok(ToDto(role));
    }

    public async Task<ServiceResult<object>> DeleteAsync(int roleId)
    {
        var role = await _slowFitContext.RoleUsers.FirstOrDefaultAsync(r => r.RoleId == roleId);
        if (role == null)
        {
            return ServiceResult<object>.NotFound("role_not_found", "Ruolo non trovato.");
        }

        var roleHasUsers = await _slowFitContext.Users.AnyAsync(u => u.RoleId == roleId);
        if (roleHasUsers)
        {
            return ServiceResult<object>.Conflict("role_in_use", "Non puoi eliminare questo ruolo perché è assegnato ad alcuni utenti.");
        }

        _slowFitContext.RoleUsers.Remove(role);
        await _slowFitContext.SaveChangesAsync();

        return ServiceResult<object>.NoContent();
    }

    private static ServiceResult<RoleDto>? Validate(RoleUpsertRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RoleName))
        {
            return ServiceResult<RoleDto>.BadRequest("invalid_role", "Il nome del ruolo è obbligatorio.");
        }

        return null;
    }

    private static RoleDto ToDto(RoleUser role) => new()
    {
        RoleId = role.RoleId,
        RoleName = role.RoleName
    };
}

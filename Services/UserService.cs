using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public sealed class UserService(SlowFitContext slowFitContext) : IUserService
{
    private readonly SlowFitContext _slowFitContext = slowFitContext;

    public async Task<ServiceResult<IReadOnlyList<UserRes>>> GetUsersByRoleAsync(int roleId)
    {
        var roleExists = await _slowFitContext.RoleUsers.AnyAsync(r => r.RoleId == roleId);
        if (!roleExists) return ServiceResult<IReadOnlyList<UserRes>>.NotFound("role_not_found", "Role not found.");

        var users = await _slowFitContext.Users.Where(u => u.RoleId == roleId).Select(u => ToUserRes(u)).ToListAsync();
        return ServiceResult<IReadOnlyList<UserRes>>.Ok(users);
    }

    public async Task<ServiceResult<IReadOnlyList<UserRes>>> GetUsersByPtIdAsync(int ptId)
    {
        var users = await _slowFitContext.Users.Where(u => u.PtId == ptId).Select(u => ToUserRes(u)).ToListAsync();
        return users.Count == 0 ? ServiceResult<IReadOnlyList<UserRes>>.NotFound("users_not_found", "No users found for the given PT.") : ServiceResult<IReadOnlyList<UserRes>>.Ok(users);
    }

    public async Task<ServiceResult<IReadOnlyList<UserProfile>>> GetAllUsersAsync()
    {
        var users = await _slowFitContext.Users.Select(u => ToUserProfile(u)).ToListAsync();
        return users.Count == 0 ? ServiceResult<IReadOnlyList<UserProfile>>.NoContent() : ServiceResult<IReadOnlyList<UserProfile>>.Ok(users);
    }

    public async Task<ServiceResult<UserRes>> GetProfileAsync(int userId)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        return user == null ? ServiceResult<UserRes>.NotFound("user_not_found", "Profile not found.") : ServiceResult<UserRes>.Ok(ToUserRes(user));
    }

    public async Task<ServiceResult<UserProfile>> GetProfileByEmailAsync(string email)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user == null ? ServiceResult<UserProfile>.NotFound("user_not_found", "Profile not found.") : ServiceResult<UserProfile>.Ok(ToUserProfile(user));
    }

    public async Task<ServiceResult<object>> CreateProfileAsync(int userId, AddProfile request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.Country) || string.IsNullOrWhiteSpace(request.Province) || request.ZipCode == 0 || request.BirthDate == null)
        {
            return ServiceResult<object>.BadRequest("invalid_profile", "Missing required profile fields.");
        }

        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return ServiceResult<object>.NotFound("user_not_found", "User not found.");

        user.Address = request.Address;
        user.City = request.City;
        user.Province = request.Province;
        user.Country = request.Country;
        user.ZipCode = request.ZipCode;
        user.ImageProfile = request.ImageProfile;
        user.BirthDate = request.BirthDate?.Date;
        user.Phone = request.Phone;

        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> UpdateUserAsync(int userId, UserProfile request)
    {
        if (request == null || request.UserId != userId)
        {
            return ServiceResult<object>.BadRequest("invalid_user", "Invalid data or ID does not match.");
        }

        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return ServiceResult<object>.NotFound("user_not_found", "User not found.");

        user.FirstName = request.FirstName;
        user.Surname = request.Surname;
        user.BirthDate = request.BirthDate?.Date;
        user.Province = request.Province;
        user.Country = request.Country;
        user.City = request.City;
        user.ZipCode = request.ZipCode;
        user.Address = request.Address;
        user.ImageProfile = request.ImageProfile;
        user.Phone = request.Phone;

        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> DeleteAsync(int userId)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return ServiceResult<object>.NotFound("user_not_found", "User not found.");

        _slowFitContext.Users.Remove(user);
        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    private static UserRes ToUserRes(User user) => new()
    {
        UserId = user.UserId,
        FirstName = user.FirstName,
        Surname = user.Surname,
        Email = user.Email,
        RoleId = user.RoleId,
        PtId = user.PtId,
        Phone = user.Phone,
        ImageProfile = user.ImageProfile
    };

    private static UserProfile ToUserProfile(User user) => new()
    {
        UserId = user.UserId,
        FirstName = user.FirstName,
        Surname = user.Surname,
        Email = user.Email,
        Address = user.Address,
        City = user.City,
        Country = user.Country,
        Province = user.Province,
        ZipCode = user.ZipCode,
        BirthDate = user.BirthDate,
        RoleId = user.RoleId,
        ImageProfile = user.ImageProfile,
        Phone = user.Phone
    };
}

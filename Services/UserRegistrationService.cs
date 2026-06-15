using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public sealed class UserRegistrationService(SlowFitContext slowFitContext) : IUserRegistrationService
{
    private readonly SlowFitContext _slowFitContext = slowFitContext;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<ServiceResult<UserRegisterResponse>> RegisterAsync(UserRegister request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.RoleId <= 0)
        {
            return ServiceResult<UserRegisterResponse>.BadRequest("invalid_registration", "All fields are required.");
        }

        if (await _slowFitContext.Users.AnyAsync(u => u.Email == request.Email))
        {
            return ServiceResult<UserRegisterResponse>.Conflict("email_in_use", "Email is already in use.");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            Surname = request.Surname,
            Email = request.Email,
            RoleId = request.RoleId,
            PtId = request.PtId
        };

        user.Password = _passwordHasher.HashPassword(user, request.Password);

        _slowFitContext.Users.Add(user);
        await _slowFitContext.SaveChangesAsync();

        return ServiceResult<UserRegisterResponse>.Created(new UserRegisterResponse
        {
            UserId = user.UserId,
            RoleId = user.RoleId ?? 0,
            Message = "New user has been added successfully!"
        });
    }
}

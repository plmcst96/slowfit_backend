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
        var users = await _slowFitContext.Users.Where(u => u.RoleId == roleId).Select(u => ToUserRes(u)).ToListAsync();
        return ServiceResult<IReadOnlyList<UserRes>>.Ok(users);
    }

    public async Task<ServiceResult<IReadOnlyList<UserRes>>> GetUsersByPtIdAsync(int ptId)
    {
        var users = await _slowFitContext.Users.Where(u => u.PtId == ptId).Select(u => ToUserRes(u)).ToListAsync();
        return ServiceResult<IReadOnlyList<UserRes>>.Ok(users);
    }

    public async Task<ServiceResult<IReadOnlyList<UserProfile>>> GetAllUsersAsync()
    {
        var users = await _slowFitContext.Users.Select(u => ToUserProfile(u)).ToListAsync();
        return ServiceResult<IReadOnlyList<UserProfile>>.Ok(users);
    }

    public async Task<ServiceResult<UserRes>> GetProfileAsync(int userId)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        return user == null ? ServiceResult<UserRes>.Ok(default!) : ServiceResult<UserRes>.Ok(ToUserRes(user));
    }

    public async Task<ServiceResult<UserProfile>> GetProfileByEmailAsync(string email)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user == null ? ServiceResult<UserProfile>.Ok(default!) : ServiceResult<UserProfile>.Ok(ToUserProfile(user));
    }

    public async Task<ServiceResult<object>> CreateProfileAsync(int userId, AddProfile request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.Country) || string.IsNullOrWhiteSpace(request.Province) || request.ZipCode == 0 || request.BirthDate == null)
        {
            return ServiceResult<object>.BadRequest("invalid_profile", "Compila tutti i campi obbligatori del profilo.");
        }

        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return ServiceResult<object>.NotFound("user_not_found", "Utente non trovato.");

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
            return ServiceResult<object>.BadRequest("invalid_user", "I dati utente non sono validi.");
        }

        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return ServiceResult<object>.NotFound("user_not_found", "Utente non trovato.");

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
        if (user == null) return ServiceResult<object>.NotFound("user_not_found", "Utente non trovato.");

        await using var transaction = await _slowFitContext.Database.BeginTransactionAsync();

        var nutritionIds = await _slowFitContext.Nutritions
            .Where(n => n.UserId == userId)
            .Select(n => n.NutritionId)
            .ToListAsync();

        var trainingIds = await _slowFitContext.Training
            .Where(t => t.UserId == userId)
            .Select(t => t.TrainingId)
            .ToListAsync();

        var orderIds = await _slowFitContext.Orders
            .Where(o => o.UserId == userId)
            .Select(o => o.OrderId)
            .ToListAsync();

        await _slowFitContext.NotificationsFires.Where(n => n.ReceiverId == userId).ExecuteDeleteAsync();
        await _slowFitContext.Appointments.Where(a => a.UserId == userId || a.PtId == userId).ExecuteDeleteAsync();
        await _slowFitContext.Measures.Where(m => m.UserId == userId).ExecuteDeleteAsync();
        await _slowFitContext.ResponseQuizzes.Where(r => r.UserId == userId).ExecuteDeleteAsync();
        await _slowFitContext.ProgressTrainings.Where(p => p.UserId == userId || trainingIds.Contains(p.TrainingId)).ExecuteDeleteAsync();
        await _slowFitContext.ProgressNutritions.Where(p => p.UserId == userId || nutritionIds.Contains(p.NutritionId)).ExecuteDeleteAsync();

        if (trainingIds.Count > 0)
        {
            await _slowFitContext.DetailExercises.Where(d => trainingIds.Contains(d.TrainingId)).ExecuteDeleteAsync();
        }

        await _slowFitContext.Training.Where(t => t.UserId == userId).ExecuteDeleteAsync();

        if (nutritionIds.Count > 0)
        {
            await _slowFitContext.NutritionMeals.Where(nm => nutritionIds.Contains(nm.NutritionId)).ExecuteDeleteAsync();
        }

        await _slowFitContext.Nutritions.Where(n => n.UserId == userId).ExecuteDeleteAsync();

        if (orderIds.Count > 0)
        {
            await _slowFitContext.Billings.Where(b => orderIds.Contains(b.OrderId)).ExecuteDeleteAsync();
        }

        await _slowFitContext.Billings.Where(b => b.UserId == userId).ExecuteDeleteAsync();
        await _slowFitContext.Orders.Where(o => o.UserId == userId).ExecuteDeleteAsync();

        await _slowFitContext.Users
            .Where(u => u.PtId == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.PtId, (int?)null));

        _slowFitContext.Users.Remove(user);
        await _slowFitContext.SaveChangesAsync();
        await transaction.CommitAsync();

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

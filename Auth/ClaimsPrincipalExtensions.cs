using System.Security.Claims;

namespace slowfit.Auth;

public static class ClaimsPrincipalExtensions
{
    public const int UserRoleId = 1;
    public const int PersonalTrainerRoleId = 2;
    public const int SuperAdminRoleId = 3;

    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("userId");
        return int.TryParse(value, out var userId) ? userId : null;
    }

    public static int? GetRoleId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("roleId") ?? user.FindFirstValue(ClaimTypes.Role);
        return int.TryParse(value, out var roleId) ? roleId : null;
    }

    public static bool IsSuperAdmin(this ClaimsPrincipal user)
        => user.GetRoleId() == SuperAdminRoleId;

    public static bool IsPersonalTrainer(this ClaimsPrincipal user)
        => user.GetRoleId() == PersonalTrainerRoleId || user.IsSuperAdmin();

    public static bool CanAccessUser(this ClaimsPrincipal user, int userId)
        => user.IsSuperAdmin() || user.IsPersonalTrainer() || user.GetUserId() == userId;
}

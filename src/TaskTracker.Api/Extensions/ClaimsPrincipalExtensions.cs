using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using TaskTracker.Application.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdValue = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }

    public static UserRole? GetUserRole(this ClaimsPrincipal principal)
    {
        var roleValue = principal.FindFirstValue(JwtClaimNames.Role);

        return Enum.TryParse<UserRole>(roleValue, ignoreCase: true, out var role) ? role : null;
    }
}
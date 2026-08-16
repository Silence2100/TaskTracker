using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaskTracker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdValue = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using TaskTracker.Application.Common;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Login.Value),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtClaimNames.Role, ((int)user.Role).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer32),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey);

        var securityKey = new SymmetricSecurityKey(keyBytes);

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.ExpirationMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
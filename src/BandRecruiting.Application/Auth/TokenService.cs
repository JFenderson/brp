using BandRecruiting.Application.Auth;
using BandRecruiting.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BandRecruiting.Infrastructure.Auth;

public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;

    private string Issuer => _config["Jwt:Issuer"]!;
    private string Audience => _config["Jwt:Audience"]!;
    private string Key => _config["Jwt:Key"]!;
    private int AccessMinutes => int.Parse(_config["Jwt:AccessTokenLifetimeMinutes"]!);
    private int RefreshDays => int.Parse(_config["Jwt:RefreshTokenLifetimeDays"]!);

    public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
    {
        _config = config;
        _userManager = userManager;
    }

    public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(
        ApplicationUser user,
        CancellationToken ct = default)
    {
        // ── ACCESS ────────────────────────────────────────────────────────────────
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var accessToken = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(AccessMinutes),
            signingCredentials: creds);

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

        // ── REFRESH (stored in AspNetUserTokens) ─────────────────────────────────
        var refreshToken = Guid.NewGuid().ToString("N");
        await _userManager.SetAuthenticationTokenAsync(
            user, Issuer, "RefreshToken", refreshToken);

        return (accessTokenString, refreshToken);
    }

    public Task InvalidateRefreshTokenAsync(ApplicationUser user, string refreshToken) =>
        _userManager.RemoveAuthenticationTokenAsync(user, Issuer, "RefreshToken");

    public async Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        var stored = await _userManager.GetAuthenticationTokenAsync(
            user, Issuer, "RefreshToken");
        return stored == refreshToken;
    }
}

using BandRecruiting.Core.Identity;

namespace BandRecruiting.Application.Auth;

public interface ITokenService
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(
        ApplicationUser user,
        CancellationToken ct = default);
    Task InvalidateRefreshTokenAsync(ApplicationUser user, string refreshToken);
    Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken);
}

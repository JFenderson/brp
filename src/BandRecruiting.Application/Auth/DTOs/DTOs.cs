namespace BandRecruiting.Application.Auth.DTOs
{
    public record RegisterStudentDto(string Email, string Password);
    public record RegisterRecruiterDto(string Email, string Password);
    public record LoginDto(string Email, string Password);
    public record RefreshTokenRequestDto(string RefreshToken);
}

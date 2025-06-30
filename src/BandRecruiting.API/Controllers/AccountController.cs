using BandRecruiting.Application.Auth;
using BandRecruiting.Application.Auth.DTOs;
using BandRecruiting.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BandRecruiting.API.Controllers
{
    [ApiController]
    [Route("account")]
    public sealed class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly RoleManager<IdentityRole<Guid>> _roleMgr;
        private readonly ITokenService _tokenSvc;

        public AccountController(
            UserManager<ApplicationUser> userMgr,
            RoleManager<IdentityRole<Guid>> roleMgr,
            ITokenService tokenSvc)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
            _tokenSvc = tokenSvc;
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userMgr.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return ValidationProblem(result);

            await EnsureRoleAsync("Student");
            await _userMgr.AddToRoleAsync(user, "Student");

            var tokens = await _tokenSvc.GenerateTokensAsync(user);
            return CreatedAtAction(nameof(Login), new { dto.Email }, tokens);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpGet("recruiter/ping")]
        public IActionResult PingRecruiter() => Ok("pong");


        [HttpPost("register/recruiter")]
        public async Task<IActionResult> RegisterRecruiter(RegisterRecruiterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userMgr.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return ValidationProblem(result);

            await EnsureRoleAsync("Recruiter");
            await _userMgr.AddToRoleAsync(user, "Recruiter");

            var tokens = await _tokenSvc.GenerateTokensAsync(user);
            return CreatedAtAction(nameof(Login), new { dto.Email }, tokens);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userMgr.FindByEmailAsync(dto.Email);
            if (user is null || !await _userMgr.CheckPasswordAsync(user, dto.Password))
                return Unauthorized();

            var tokens = await _tokenSvc.GenerateTokensAsync(user);
            return Ok(tokens);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
        {
            var user = await _userMgr.FindByLoginAsync("BRApp", dto.RefreshToken);
            if (user is null) return Unauthorized();

            if (!await _tokenSvc.ValidateRefreshTokenAsync(user, dto.RefreshToken))
                return Unauthorized();

            await _tokenSvc.InvalidateRefreshTokenAsync(user, dto.RefreshToken);
            var tokens = await _tokenSvc.GenerateTokensAsync(user);
            return Ok(tokens);
        }

        // helpers ------------------------------------------------------------
        private async Task EnsureRoleAsync(string role)
        {
            if (!await _roleMgr.RoleExistsAsync(role))
                await _roleMgr.CreateAsync(new IdentityRole<Guid>(role));
        }

        private IActionResult ValidationProblem(IdentityResult result)
        {
            var details = new ValidationProblemDetails
            {
                Title = "Registration failed",
                Errors = result.Errors
                             .GroupBy(e => e.Code)
                             .ToDictionary(
                                 g => g.Key,
                                 g => g.Select(e => e.Description).ToArray())
            };

            return ValidationProblem(details);   // built-in ControllerBase helper
        }
    }
}
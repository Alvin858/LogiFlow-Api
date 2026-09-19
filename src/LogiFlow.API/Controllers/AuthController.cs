using LogiFlow.API.Extensions;
using LogiFlow.Application.DTOs.Auth;
using LogiFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct) => Ok(await service.RegisterAsync(request, ct));
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct) => Ok(await service.LoginAsync(request, ct));
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request, CancellationToken ct) => Ok(await service.RefreshTokenAsync(request, ct));
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct) { await service.LogoutAsync(request, ct); return NoContent(); }
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> Forgot(ForgotPasswordRequest request, CancellationToken ct)
    {
        var token = await service.ForgotPasswordAsync(request, ct);
        return Ok(new { message = "If the account exists, a password reset token has been generated.", resetToken = token });
    }
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> Reset(ResetPasswordRequest request, CancellationToken ct) { await service.ResetPasswordAsync(request, ct); return Ok(new { message = "Password reset successfully." }); }
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> Change(ChangePasswordRequest request, CancellationToken ct) { await service.ChangePasswordAsync(User.GetUserId(), request, ct); return Ok(new { message = "Password changed successfully." }); }
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken ct) => Ok(await service.GetMeAsync(User.GetUserId(), ct));
}

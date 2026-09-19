using LogiFlow.Application.DTOs.Auth;

namespace LogiFlow.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct);
    Task LogoutAsync(LogoutRequest request, CancellationToken ct);
    Task<string?> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct);
    Task<UserResponse> GetMeAsync(int userId, CancellationToken ct);
}

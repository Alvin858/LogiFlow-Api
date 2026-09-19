namespace LogiFlow.Application.DTOs.Auth;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string? PhoneNumber, string? CompanyName);
public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string ResetToken, string NewPassword);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record UserResponse(int Id, string FirstName, string LastName, string Email, string? PhoneNumber, string Role, bool IsActive);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc, UserResponse User);

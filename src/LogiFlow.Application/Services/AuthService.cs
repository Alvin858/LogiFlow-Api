using LogiFlow.Application.DTOs.Auth;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class AuthService(IApplicationDbContext db, ISecurityService security) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email, ct)) throw new ConflictException("Email is already registered.");
        var role = await db.Roles.SingleOrDefaultAsync(x => x.Name == RoleNames.Customer, ct) ?? throw new InvalidOperationException("Customer role is not configured.");
        var user = new User { FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(), Email = email, PhoneNumber = request.PhoneNumber?.Trim(), RoleId = role.Id, IsActive = true };
        user.PasswordHash = security.HashPassword(user, request.Password);
        db.Users.Add(user);
        db.Customers.Add(new Customer { User = user, CompanyName = string.IsNullOrWhiteSpace(request.CompanyName) ? "Individual Customer" : request.CompanyName.Trim(), ContactPerson = $"{user.FirstName} {user.LastName}" });
        await db.SaveChangesAsync(ct);
        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Email == email, ct);
        if (user is null || !security.VerifyPassword(user, user.PasswordHash, request.Password) || !user.IsActive) throw new UnauthorizedException("Invalid email or password.");
        user.LastLoginAtUtc = DateTime.UtcNow;
        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var hash = security.HashToken(request.RefreshToken);
        var token = await db.RefreshTokens.Include(x => x.User).ThenInclude(x => x.Role).SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (token is null || !token.IsActive || !token.User.IsActive) throw new UnauthorizedException("Refresh token is invalid or expired.");
        token.RevokedAtUtc = DateTime.UtcNow;
        return await IssueTokensAsync(token.User, ct, hash);
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken ct)
    {
        var hash = security.HashToken(request.RefreshToken);
        var token = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (token is not null && token.RevokedAtUtc is null) { token.RevokedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); }
    }

    public async Task<string?> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email, ct);
        if (user is null || !user.IsActive) return null;
        var token = security.GenerateSecureToken();
        user.PasswordResetTokenHash = security.HashToken(token);
        user.PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(15);
        await db.SaveChangesAsync(ct);
        return token;
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var hash = security.HashToken(request.ResetToken);
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email && x.PasswordResetTokenHash == hash, ct);
        if (user is null || user.PasswordResetTokenExpiresAtUtc <= DateTime.UtcNow) throw new BadRequestException("Reset token is invalid or expired.");
        user.PasswordHash = security.HashPassword(user, request.NewPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAtUtc = null;
        foreach (var token in await db.RefreshTokens.Where(x => x.UserId == user.Id && x.RevokedAtUtc == null).ToListAsync(ct)) token.RevokedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Id == userId, ct) ?? throw new NotFoundException("User not found.");
        if (!security.VerifyPassword(user, user.PasswordHash, request.CurrentPassword)) throw new BadRequestException("Current password is incorrect.");
        user.PasswordHash = security.HashPassword(user, request.NewPassword);
        foreach (var token in await db.RefreshTokens.Where(x => x.UserId == userId && x.RevokedAtUtc == null).ToListAsync(ct)) token.RevokedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task<UserResponse> GetMeAsync(int userId, CancellationToken ct)
    {
        var user = await db.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == userId, ct) ?? throw new NotFoundException("User not found.");
        return ToResponse(user);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct, string? replacedHash = null)
    {
        if (user.Role is null) user.Role = await db.Roles.SingleAsync(x => x.Id == user.RoleId, ct);
        var refresh = security.GenerateSecureToken();
        var refreshHash = security.HashToken(refresh);
        if (replacedHash is not null) { var old = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == replacedHash, ct); if (old is not null) old.ReplacedByTokenHash = refreshHash; }
        db.RefreshTokens.Add(new RefreshToken { TokenHash = refreshHash, ExpiresAtUtc = DateTime.UtcNow.AddDays(7), UserId = user.Id });
        await db.SaveChangesAsync(ct);
        return new AuthResponse(security.GenerateAccessToken(user), refresh, security.AccessTokenExpiresAtUtc, ToResponse(user));
    }

    private static UserResponse ToResponse(User user) => new(user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.Role.Name, user.IsActive);
}

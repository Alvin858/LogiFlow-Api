using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Interfaces;

public interface ISecurityService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
    string GenerateSecureToken();
    string HashToken(string token);
    string GenerateAccessToken(User user);
    DateTime AccessTokenExpiresAtUtc { get; }
}

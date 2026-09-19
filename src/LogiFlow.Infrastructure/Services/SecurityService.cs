using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LogiFlow.Infrastructure.Services;

public class SecurityService(IConfiguration configuration, IPasswordHasher<User> passwordHasher) : ISecurityService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    public DateTime AccessTokenExpiresAtUtc { get; private set; }

    public string HashPassword(User user, string password) => _passwordHasher.HashPassword(user, password);
    public bool VerifyPassword(User user, string hashedPassword, string providedPassword) => _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
    public string GenerateSecureToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    public string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    public string GenerateAccessToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
        var minutes = _configuration.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
        AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(minutes);
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"), new Claim(ClaimTypes.Role, user.Role.Name) };
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(issuer, audience, claims, expires: AccessTokenExpiresAtUtc, signingCredentials: credentials));
    }
}

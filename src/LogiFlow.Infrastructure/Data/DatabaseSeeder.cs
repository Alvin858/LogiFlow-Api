using LogiFlow.Domain.Constants;
using LogiFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher<User> hasher, string email, string password, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);
        var adminRole = await db.Roles.SingleAsync(x => x.Name == RoleNames.Admin, ct);
        if (await db.Users.AnyAsync(x => x.RoleId == adminRole.Id, ct)) return;
        var user = new User { FirstName = "System", LastName = "Administrator", Email = email.Trim().ToLowerInvariant(), RoleId = adminRole.Id, IsActive = true };
        user.PasswordHash = hasher.HashPassword(user, password);
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }
}

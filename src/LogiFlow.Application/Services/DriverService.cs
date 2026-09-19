using LogiFlow.Application.DTOs.Drivers;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class DriverService(IApplicationDbContext db, ISecurityService security) : IDriverService
{
    public async Task<IReadOnlyCollection<DriverResponse>> GetAllAsync(CancellationToken ct) => await db.Drivers.AsNoTracking().Include(x => x.User).OrderBy(x => x.User.LastName).ThenBy(x => x.User.FirstName).Select(x => new DriverResponse(x.Id, x.UserId, x.User.FirstName, x.User.LastName, x.User.Email, x.User.PhoneNumber, x.LicenseNumber, x.LicenseExpiryDate, x.ExperienceYears, x.IsAvailable, x.User.IsActive)).ToListAsync(ct);
    public async Task<DriverResponse> GetByIdAsync(int id, CancellationToken ct) => Map(await db.Drivers.AsNoTracking().Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Driver not found."));
    public async Task<DriverResponse> CreateAsync(CreateDriverRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email, ct)) throw new ConflictException("Email is already registered.");
        if (await db.Drivers.AnyAsync(x => x.LicenseNumber == request.LicenseNumber.Trim(), ct)) throw new ConflictException("License number already exists.");
        var role = await db.Roles.SingleOrDefaultAsync(x => x.Name == RoleNames.Driver, ct) ?? throw new InvalidOperationException("Driver role is not configured.");
        var user = new User { FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(), Email = email, PhoneNumber = request.PhoneNumber?.Trim(), RoleId = role.Id, IsActive = true };
        user.PasswordHash = security.HashPassword(user, request.Password);
        var driver = new Driver { User = user, LicenseNumber = request.LicenseNumber.Trim(), LicenseExpiryDate = request.LicenseExpiryDate, ExperienceYears = request.ExperienceYears, IsAvailable = request.IsAvailable };
        db.Users.Add(user); db.Drivers.Add(driver); await db.SaveChangesAsync(ct);
        return Map(driver);
    }
    public async Task<DriverResponse> UpdateAsync(int id, UpdateDriverRequest request, CancellationToken ct)
    {
        var d = await db.Drivers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Driver not found.");
        var license = request.LicenseNumber.Trim();
        if (await db.Drivers.AnyAsync(x => x.Id != id && x.LicenseNumber == license, ct)) throw new ConflictException("License number already exists.");
        d.User.FirstName = request.FirstName.Trim(); d.User.LastName = request.LastName.Trim(); d.User.PhoneNumber = request.PhoneNumber?.Trim(); d.LicenseNumber = license; d.LicenseExpiryDate = request.LicenseExpiryDate; d.ExperienceYears = request.ExperienceYears; d.IsAvailable = request.IsAvailable; d.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return Map(d);
    }
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var d = await db.Drivers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Driver not found.");
        d.User.IsActive = false; d.IsAvailable = false; await db.SaveChangesAsync(ct);
    }
    public async Task<DriverResponse> UpdateAvailabilityAsync(int id, UpdateDriverAvailabilityRequest request, CancellationToken ct)
    {
        var d = await db.Drivers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Driver not found.");
        if (request.IsAvailable && d.LicenseExpiryDate.Date < DateTime.UtcNow.Date) throw new BadRequestException("Driver license has expired.");
        d.IsAvailable = request.IsAvailable; d.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return Map(d);
    }
    private static DriverResponse Map(Driver x) => new(x.Id, x.UserId, x.User.FirstName, x.User.LastName, x.User.Email, x.User.PhoneNumber, x.LicenseNumber, x.LicenseExpiryDate, x.ExperienceYears, x.IsAvailable, x.User.IsActive);
}

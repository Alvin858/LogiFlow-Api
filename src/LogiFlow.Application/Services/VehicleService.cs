using LogiFlow.Application.DTOs.Vehicles;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class VehicleService(IApplicationDbContext db) : IVehicleService
{
    public async Task<IReadOnlyCollection<VehicleResponse>> GetAllAsync(CancellationToken ct) => await db.Vehicles.AsNoTracking().OrderBy(x => x.RegistrationNumber).Select(MapExpression()).ToListAsync(ct);
    public async Task<VehicleResponse> GetByIdAsync(int id, CancellationToken ct) => Map(await db.Vehicles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Vehicle not found."));
    public async Task<VehicleResponse> CreateAsync(CreateVehicleRequest request, CancellationToken ct)
    {
        var reg = request.RegistrationNumber.Trim().ToUpperInvariant();
        if (await db.Vehicles.AnyAsync(x => x.RegistrationNumber == reg, ct)) throw new ConflictException("Vehicle registration number already exists.");
        var v = new Vehicle { VehicleType = request.VehicleType.Trim(), RegistrationNumber = reg, CapacityKg = request.CapacityKg, Status = request.Status, InsuranceExpiryDate = request.InsuranceExpiryDate, FitnessExpiryDate = request.FitnessExpiryDate };
        db.Vehicles.Add(v); await db.SaveChangesAsync(ct); return Map(v);
    }
    public async Task<VehicleResponse> UpdateAsync(int id, UpdateVehicleRequest request, CancellationToken ct)
    {
        var v = await db.Vehicles.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Vehicle not found.");
        var reg = request.RegistrationNumber.Trim().ToUpperInvariant();
        if (await db.Vehicles.AnyAsync(x => x.Id != id && x.RegistrationNumber == reg, ct)) throw new ConflictException("Vehicle registration number already exists.");
        v.VehicleType = request.VehicleType.Trim(); v.RegistrationNumber = reg; v.CapacityKg = request.CapacityKg; v.Status = request.Status; v.InsuranceExpiryDate = request.InsuranceExpiryDate; v.FitnessExpiryDate = request.FitnessExpiryDate; v.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return Map(v);
    }
    public async Task DeleteAsync(int id, CancellationToken ct) { var v = await db.Vehicles.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Vehicle not found."); db.Vehicles.Remove(v); await db.SaveChangesAsync(ct); }
    public async Task<VehicleResponse> UpdateStatusAsync(int id, UpdateVehicleStatusRequest request, CancellationToken ct) { var v = await db.Vehicles.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Vehicle not found."); v.Status = request.Status; v.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return Map(v); }
    private static VehicleResponse Map(Vehicle x) => new(x.Id, x.VehicleType, x.RegistrationNumber, x.CapacityKg, x.Status, x.InsuranceExpiryDate, x.FitnessExpiryDate, x.CreatedAtUtc);
    private static System.Linq.Expressions.Expression<Func<Vehicle, VehicleResponse>> MapExpression() => x => new VehicleResponse(x.Id, x.VehicleType, x.RegistrationNumber, x.CapacityKg, x.Status, x.InsuranceExpiryDate, x.FitnessExpiryDate, x.CreatedAtUtc);
}

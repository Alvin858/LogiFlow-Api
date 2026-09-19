using LogiFlow.Application.DTOs.Customers;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class CustomerService(IApplicationDbContext db) : ICustomerService
{
    public async Task<CustomerResponse> GetProfileAsync(int userId, CancellationToken ct) => await GetByUserIdAsync(userId, ct);

    public async Task<CustomerResponse> UpdateProfileAsync(int userId, UpdateCustomerProfileRequest request, CancellationToken ct)
    {
        var customer = await db.Customers.Include(x => x.User).Include(x => x.Addresses).SingleOrDefaultAsync(x => x.UserId == userId, ct) ?? throw new NotFoundException("Customer profile not found.");
        customer.CompanyName = request.CompanyName.Trim(); customer.ContactPerson = request.ContactPerson.Trim(); customer.TaxNumber = request.TaxNumber?.Trim();
        customer.User.PhoneNumber = request.PhoneNumber?.Trim();
        await db.SaveChangesAsync(ct);
        return Map(customer);
    }

    public async Task<IReadOnlyCollection<AddressResponse>> GetAddressesAsync(int userId, CancellationToken ct)
    {
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.UserId == userId, ct) ?? throw new NotFoundException("Customer profile not found.");
        return await db.CustomerAddresses.Where(x => x.CustomerId == customer.Id).OrderByDescending(x => x.IsDefault).ThenBy(x => x.Id).Select(x => new AddressResponse(x.Id, x.AddressLine1, x.AddressLine2, x.City, x.State, x.PostalCode, x.Country, x.IsDefault)).ToListAsync(ct);
    }

    public async Task<AddressResponse> AddAddressAsync(int userId, CreateAddressRequest request, CancellationToken ct)
    {
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.UserId == userId, ct) ?? throw new NotFoundException("Customer profile not found.");
        if (request.IsDefault) await ClearDefaultAsync(customer.Id, ct);
        var address = new CustomerAddress { CustomerId = customer.Id, AddressLine1 = request.AddressLine1.Trim(), AddressLine2 = request.AddressLine2?.Trim(), City = request.City.Trim(), State = request.State.Trim(), PostalCode = request.PostalCode.Trim(), Country = request.Country.Trim(), IsDefault = request.IsDefault };
        if (!await db.CustomerAddresses.AnyAsync(x => x.CustomerId == customer.Id, ct)) address.IsDefault = true;
        db.CustomerAddresses.Add(address); await db.SaveChangesAsync(ct);
        return new AddressResponse(address.Id, address.AddressLine1, address.AddressLine2, address.City, address.State, address.PostalCode, address.Country, address.IsDefault);
    }

    public async Task<AddressResponse> UpdateAddressAsync(int userId, int addressId, UpdateAddressRequest request, CancellationToken ct)
    {
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.UserId == userId, ct) ?? throw new NotFoundException("Customer profile not found.");
        var address = await db.CustomerAddresses.SingleOrDefaultAsync(x => x.Id == addressId && x.CustomerId == customer.Id, ct) ?? throw new NotFoundException("Address not found.");
        if (request.IsDefault) await ClearDefaultAsync(customer.Id, ct, addressId);
        address.AddressLine1 = request.AddressLine1.Trim(); address.AddressLine2 = request.AddressLine2?.Trim(); address.City = request.City.Trim(); address.State = request.State.Trim(); address.PostalCode = request.PostalCode.Trim(); address.Country = request.Country.Trim(); address.IsDefault = request.IsDefault;
        await db.SaveChangesAsync(ct);
        return new AddressResponse(address.Id, address.AddressLine1, address.AddressLine2, address.City, address.State, address.PostalCode, address.Country, address.IsDefault);
    }

    public async Task DeleteAddressAsync(int userId, int addressId, CancellationToken ct)
    {
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.UserId == userId, ct) ?? throw new NotFoundException("Customer profile not found.");
        var address = await db.CustomerAddresses.SingleOrDefaultAsync(x => x.Id == addressId && x.CustomerId == customer.Id, ct) ?? throw new NotFoundException("Address not found.");
        var wasDefault = address.IsDefault; db.CustomerAddresses.Remove(address); await db.SaveChangesAsync(ct);
        if (wasDefault) { var replacement = await db.CustomerAddresses.Where(x => x.CustomerId == customer.Id).OrderBy(x => x.Id).FirstOrDefaultAsync(ct); if (replacement is not null) { replacement.IsDefault = true; await db.SaveChangesAsync(ct); } }
    }

    public async Task<CustomerResponse> GetByIdAsync(int customerId, CancellationToken ct)
    {
        var customer = await db.Customers.Include(x => x.User).Include(x => x.Addresses).SingleOrDefaultAsync(x => x.Id == customerId, ct) ?? throw new NotFoundException("Customer not found.");
        return Map(customer);
    }

    private async Task<CustomerResponse> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        var customer = await db.Customers.Include(x => x.User).Include(x => x.Addresses).SingleOrDefaultAsync(x => x.UserId == userId, ct) ?? throw new NotFoundException("Customer profile not found.");
        return Map(customer);
    }
    private async Task ClearDefaultAsync(int customerId, CancellationToken ct, int? exceptId = null)
    {
        var addresses = await db.CustomerAddresses.Where(x => x.CustomerId == customerId && x.IsDefault && (!exceptId.HasValue || x.Id != exceptId.Value)).ToListAsync(ct);
        foreach (var a in addresses) a.IsDefault = false;
    }
    private static CustomerResponse Map(Customer c) => new(c.Id, c.UserId, c.CompanyName, c.ContactPerson, c.TaxNumber, c.User.Email, c.User.PhoneNumber, c.Addresses.OrderByDescending(x => x.IsDefault).ThenBy(x => x.Id).Select(x => new AddressResponse(x.Id, x.AddressLine1, x.AddressLine2, x.City, x.State, x.PostalCode, x.Country, x.IsDefault)).ToList());
}

using LogiFlow.API.Extensions;
using LogiFlow.Application.DTOs.Customers;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<ActionResult<CustomerResponse>> Profile(CancellationToken ct) => Ok(await service.GetProfileAsync(User.GetUserId(), ct));
    [HttpPut("profile")]
    public async Task<ActionResult<CustomerResponse>> UpdateProfile(UpdateCustomerProfileRequest request, CancellationToken ct) => Ok(await service.UpdateProfileAsync(User.GetUserId(), request, ct));
    [HttpGet("addresses")]
    public async Task<ActionResult<IReadOnlyCollection<AddressResponse>>> Addresses(CancellationToken ct) => Ok(await service.GetAddressesAsync(User.GetUserId(), ct));
    [HttpPost("addresses")]
    public async Task<ActionResult<AddressResponse>> AddAddress(CreateAddressRequest request, CancellationToken ct) => Ok(await service.AddAddressAsync(User.GetUserId(), request, ct));
    [HttpPut("addresses/{id:int}")]
    public async Task<ActionResult<AddressResponse>> UpdateAddress(int id, UpdateAddressRequest request, CancellationToken ct) => Ok(await service.UpdateAddressAsync(User.GetUserId(), id, request, ct));
    [HttpDelete("addresses/{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id, CancellationToken ct) { await service.DeleteAddressAsync(User.GetUserId(), id, ct); return NoContent(); }
    [HttpGet("{id:int}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<CustomerResponse>> GetById(int id, CancellationToken ct) => Ok(await service.GetByIdAsync(id, ct));
}

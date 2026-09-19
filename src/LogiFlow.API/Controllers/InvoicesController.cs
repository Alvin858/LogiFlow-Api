using LogiFlow.API.Extensions;
using LogiFlow.Application.DTOs.Billing;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff + "," + RoleNames.Customer)]
public class InvoicesController(IInvoiceService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceResponse>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(int id, CancellationToken ct)
    {
        var isAdminOrStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.LogisticsStaff);
        return Ok(await service.GetByIdAsync(id, isAdminOrStaff ? null : User.GetUserId(), ct));
    }

    [HttpGet("customer")]
    [Authorize(Roles = RoleNames.Customer)]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceResponse>>> CustomerInvoices(CancellationToken ct) => Ok(await service.GetCustomerInvoicesAsync(User.GetUserId(), ct));

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<InvoiceResponse>> Create(CreateInvoiceRequest request, CancellationToken ct) => Ok(await service.CreateAsync(request, ct));

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<InvoiceResponse>> Update(int id, UpdateInvoiceRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(id, request, ct));


}

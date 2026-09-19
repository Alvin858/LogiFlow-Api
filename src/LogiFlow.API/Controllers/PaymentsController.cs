using LogiFlow.API.Extensions;
using LogiFlow.Application.DTOs.Billing;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff + "," + RoleNames.Customer)]
public class PaymentsController(IInvoiceService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PaymentResponse>>> GetAll(CancellationToken ct)
    {
        var customerOnly = User.IsInRole(RoleNames.Customer);
        return Ok(await service.GetPaymentsAsync(customerOnly ? User.GetUserId() : null, ct));
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> Create(CreatePaymentRequest request, CancellationToken ct)
    {
        var customerOnly = User.IsInRole(RoleNames.Customer);
        return Ok(await service.CreatePaymentAsync(request, customerOnly ? User.GetUserId() : null, ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaymentResponse>> GetById(int id, CancellationToken ct)
    {
        var payments = await service.GetPaymentsAsync(User.IsInRole(RoleNames.Customer) ? User.GetUserId() : null, ct);
        var payment = payments.FirstOrDefault(x => x.Id == id);
        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<PaymentResponse>> UpdateStatus(int id, UpdatePaymentStatusRequest request, CancellationToken ct) => Ok(await service.UpdatePaymentStatusAsync(id, request, ct));
}

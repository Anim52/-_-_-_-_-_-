using MaxiMed.Application.Invoices;
using MaxiMed.Domain.Entities;
using MaxiMed.Domain.Lookups;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _svc;
    public InvoicesController(IInvoiceService svc) => _svc = svc;

    [HttpGet("by-appointment/{appointmentId:long}")]
    public async Task<ActionResult<InvoiceDto>> GetOrCreateByAppointment(long appointmentId, CancellationToken ct)
        => Ok(await _svc.GetOrCreateByAppointmentAsync(appointmentId, ct));

    public sealed record DiscountRequest(decimal DiscountAmount);

    [HttpPost("{invoiceId:long}/discount")]
    public async Task<IActionResult> UpdateDiscount(long invoiceId, [FromBody] DiscountRequest req, CancellationToken ct)
    {
        await _svc.UpdateDiscountAsync(invoiceId, req.DiscountAmount, ct);
        return NoContent();
    }

    public sealed record AddPaymentRequest(PaymentMethod Method, decimal Amount, DateTime PaidAt, string? Note);

    [HttpPost("{invoiceId:long}/payments")]
    public async Task<ActionResult<long>> AddPayment(long invoiceId, [FromBody] AddPaymentRequest req, CancellationToken ct)
        => Ok(await _svc.AddPaymentAsync(invoiceId, req.Method, req.Amount, req.PaidAt, req.Note, ct));

    public sealed record PaymentDateRequest(DateTime PaidAt);

    [HttpPost("payments/{paymentId:long}/date")]
    public async Task<IActionResult> UpdatePaymentDate(long paymentId, [FromBody] PaymentDateRequest req, CancellationToken ct)
    {
        await _svc.UpdatePaymentDateAsync(paymentId, req.PaidAt, ct);
        return NoContent();
    }

    [HttpDelete("payments/{paymentId:long}")]
    public async Task<IActionResult> DeletePayment(long paymentId, CancellationToken ct)
    {
        await _svc.DeletePaymentAsync(paymentId, ct);
        return NoContent();
    }
    [HttpGet("{id:long}/print")]
    public async Task<IActionResult> GetPrintData(long id)
    {
        var invoice = await _svc.GetByIdWithDetailsAsync(id);

        if (invoice == null)
            return NotFound();

        return Ok(new
        {
            invoice.Id,
            PatientName = invoice.Patient.LastName + " " +
                          invoice.Patient.FirstName + " " +
                          invoice.Patient.MiddleName,
            invoice.TotalAmount,
            invoice.DiscountAmount,
            invoice.PaidAmount,
            DueAmount = invoice.TotalAmount - invoice.DiscountAmount - invoice.PaidAmount,
            Items = invoice.Items.Select(x => new
            {
                ServiceName = x.Service != null ? x.Service.Name : "Услуга",
                x.Qty,
                x.Price,
                LineTotal = x.Qty * x.Price
            }).ToList(),
            Payments = invoice.Payments.Select(x => new
            {
                x.Amount,
                x.PaidAt,
                Method = x.Method.ToString(),
                x.Note
            }).ToList()
        });
    }
}

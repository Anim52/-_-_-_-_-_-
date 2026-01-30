using MaxiMed.Application.Invoices;
using MaxiMed.Domain.Lookups;

namespace MaxiMed.Wpf.Api;

public sealed class InvoiceApiService : IInvoiceService
{
    private readonly ApiClient _api;
    public InvoiceApiService(ApiClient api) => _api = api;

    public async Task<InvoiceDto> GetOrCreateByAppointmentAsync(long appointmentId, CancellationToken ct = default)
        => (await _api.GetAsync<InvoiceDto>($"api/invoices/by-appointment/{appointmentId}", ct))!;

    private sealed record DiscountReq(decimal DiscountAmount);

    public Task UpdateDiscountAsync(long invoiceId, decimal discountAmount, CancellationToken ct = default)
        => _api.PostAsync($"api/invoices/{invoiceId}/discount", new DiscountReq(discountAmount), ct);

    private sealed record AddPayReq(PaymentMethod Method, decimal Amount, DateTime PaidAt, string? Note);

    public async Task<long> AddPaymentAsync(long invoiceId, PaymentMethod method, decimal amount, DateTime paidAt, string? note, CancellationToken ct = default)
        => (await _api.PostAsync<long>($"api/invoices/{invoiceId}/payments", new AddPayReq(method, amount, paidAt, note), ct))!;

    private sealed record PayDateReq(DateTime PaidAt);

    public Task UpdatePaymentDateAsync(long paymentId, DateTime paidAt, CancellationToken ct = default)
        => _api.PostAsync($"api/invoices/payments/{paymentId}/date", new PayDateReq(paidAt), ct);

    public Task DeletePaymentAsync(long paymentId, CancellationToken ct = default)
        => _api.DeleteAsync($"api/invoices/payments/{paymentId}", ct);
}

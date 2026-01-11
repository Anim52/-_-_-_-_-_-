using MaxiMed.Domain.Entities;
using MaxiMed.Domain.Lookups;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Invoices
{
    public sealed class InvoiceService : IInvoiceService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public InvoiceService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<InvoiceDto> GetOrCreateByAppointmentAsync(long appointmentId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            // 1) грузим приём с услугами
            var appt = await db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Services).ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                ?? throw new InvalidOperationException("Запись не найдена");

            // 2) ищем счёт
            var invoice = await db.Invoices
                .Include(i => i.Items).ThenInclude(it => it.Service)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.AppointmentId == appointmentId, ct);

            // 3) если нет — создаём по услугам записи
            if (invoice is null)
            {
                invoice = new Invoice
                {
                    AppointmentId = appt.Id,
                    PatientId = appt.PatientId,
                    CreatedAt = DateTime.UtcNow,
                    DiscountAmount = 0,
                    PaidAmount = 0,
                    TotalAmount = 0
                };

                foreach (var s in appt.Services)
                {
                    invoice.Items.Add(new InvoiceItem
                    {
                        ServiceId = s.ServiceId,
                        Qty = s.Qty,
                        Price = s.Price
                    });
                }

                invoice.TotalAmount = invoice.Items.Sum(x => x.Qty * x.Price);

                db.Invoices.Add(invoice);
                await db.SaveChangesAsync(ct);

                // перегрузим с навигациями
                invoice = await db.Invoices
                    .Include(i => i.Items).ThenInclude(it => it.Service)
                    .Include(i => i.Payments)
                    .FirstAsync(i => i.Id == invoice.Id, ct);
            }
            else
            {
                // Можно синхронизировать Items с услугами записи при желании.
                // Пока не трогаем, чтобы не ломать оплаты.
            }

            return Map(invoice);
        }

        public async Task UpdateDiscountAsync(long invoiceId, decimal discountAmount, CancellationToken ct = default)
        {
            if (discountAmount < 0) throw new ArgumentException("Скидка не может быть отрицательной");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var inv = await db.Invoices.Include(x => x.Items).Include(x => x.Payments)
                .FirstOrDefaultAsync(x => x.Id == invoiceId, ct)
                ?? throw new InvalidOperationException("Счёт не найден");

            inv.DiscountAmount = discountAmount;
            inv.TotalAmount = inv.Items.Sum(x => x.Qty * x.Price);
            inv.PaidAmount = inv.Payments.Sum(x => x.Amount);

            await db.SaveChangesAsync(ct);
        }

        public async Task<long> AddPaymentAsync(long invoiceId, PaymentMethod method, decimal amount, string? note, CancellationToken ct = default)
        {
            if (amount <= 0) throw new ArgumentException("Сумма должна быть больше 0");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var inv = await db.Invoices
                .Include(i => i.Payments)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoiceId, ct)
                ?? throw new InvalidOperationException("Счёт не найден");

            inv.Payments.Add(new Payment
            {
                InvoiceId = invoiceId,
                Method = method,
                Amount = amount,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                PaidAt = DateTime.UtcNow
            });

            inv.TotalAmount = inv.Items.Sum(x => x.Qty * x.Price);
            inv.PaidAmount = inv.Payments.Sum(x => x.Amount);

            await db.SaveChangesAsync(ct);

            return inv.Payments.OrderByDescending(x => x.Id).First().Id;
        }

        public async Task DeletePaymentAsync(long paymentId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var p = await db.Payments.FirstOrDefaultAsync(x => x.Id == paymentId, ct);
            if (p is null) return;

            var inv = await db.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstAsync(i => i.Id == p.InvoiceId, ct);

            db.Payments.Remove(p);

            // пересчёт
            inv.TotalAmount = inv.Items.Sum(x => x.Qty * x.Price);
            inv.PaidAmount = inv.Payments.Where(x => x.Id != paymentId).Sum(x => x.Amount);

            await db.SaveChangesAsync(ct);
        }

        private static InvoiceDto Map(Invoice inv) => new()
        {
            Id = inv.Id,
            AppointmentId = inv.AppointmentId,
            PatientId = inv.PatientId,
            TotalAmount = inv.TotalAmount,
            DiscountAmount = inv.DiscountAmount,
            PaidAmount = inv.PaidAmount,
            Items = inv.Items.Select(it => new InvoiceItemDto
            {
                Id = it.Id,
                ServiceId = it.ServiceId,
                ServiceName = it.Service?.Name ?? "",
                Qty = it.Qty,
                Price = it.Price
            }).ToList(),
            Payments = inv.Payments
                .OrderByDescending(p => p.PaidAt)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    Method = p.Method,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt,
                    Note = p.Note
                }).ToList()
        };
    }
}

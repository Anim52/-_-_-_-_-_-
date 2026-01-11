using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public sealed class AppointmentServicesService : IAppointmentServicesService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public AppointmentServicesService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<AppointmentServiceItemDto>> GetAsync(long appointmentId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.AppointmentServices.AsNoTracking()
                .Where(x => x.AppointmentId == appointmentId)
                .Include(x => x.Service)
                .OrderBy(x => x.Service.Name)
                .Select(x => new AppointmentServiceItemDto
                {
                    Id = x.Id,
                    AppointmentId = x.AppointmentId,
                    ServiceId = x.ServiceId,
                    ServiceName = x.Service.Name,
                    Qty = x.Qty,
                    Price = x.Price
                })
                .ToListAsync(ct);
        }

        public async Task<long> AddAsync(long appointmentId, int serviceId, int qty = 1, decimal? price = null, CancellationToken ct = default)
        {
            if (qty <= 0) qty = 1;

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var svc = await db.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == serviceId, ct)
                      ?? throw new InvalidOperationException("Услуга не найдена");

            var item = new MaxiMed.Domain.Entities.AppointmentService
            {
                AppointmentId = appointmentId,
                ServiceId = serviceId,
                Qty = qty,
                Price = price ?? svc.BasePrice
            };

            db.AppointmentServices.Add(item);
            await db.SaveChangesAsync(ct);
            return item.Id;
        }

        public async Task UpdateAsync(AppointmentServiceItemDto dto, CancellationToken ct = default)
        {
            if (dto.Qty <= 0) throw new ArgumentException("Qty должен быть > 0");
            if (dto.Price < 0) throw new ArgumentException("Price не может быть отрицательной");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var item = await db.AppointmentServices.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                       ?? throw new InvalidOperationException("Строка услуги не найдена");

            item.Qty = dto.Qty;
            item.Price = dto.Price;

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var item = await db.AppointmentServices.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (item is null) return;

            db.AppointmentServices.Remove(item);
            await db.SaveChangesAsync(ct);
        }
    }
}

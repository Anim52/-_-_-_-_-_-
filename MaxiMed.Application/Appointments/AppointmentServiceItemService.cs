using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public sealed class AppointmentServiceItemService : IAppointmentServiceItemService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public AppointmentServiceItemService(IDbContextFactory<MaxiMedDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<AppointmentServiceItemDto>> GetAsync(long appointmentId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.AppointmentServices
                .Include(x => x.Service)
                .Where(x => x.AppointmentId == appointmentId)
                .Select(x => new AppointmentServiceItemDto
                {
                    Id = x.Id,
                    AppointmentId = x.AppointmentId,
                    ServiceId = x.ServiceId,
                    ServiceName = x.Service.Name,
                    Qty = x.Qty,
                    Price = x.Price
                })
                .ToListAsync();
        }

        public async Task AddAsync(long appointmentId, int serviceId, int qty)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var service = await db.Services.FirstAsync(x => x.Id == serviceId);

            db.AppointmentServices.Add(new AppointmentService
            {
                AppointmentId = appointmentId,
                ServiceId = serviceId,
                Qty = qty,
                Price = service.BasePrice
            });

            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(long id, int qty, decimal price)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var item = await db.AppointmentServices.FirstAsync(x => x.Id == id);
            item.Qty = qty;
            item.Price = price;

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var item = await db.AppointmentServices.FirstAsync(x => x.Id == id);
            db.AppointmentServices.Remove(item);

            await db.SaveChangesAsync();
        }
    }
}

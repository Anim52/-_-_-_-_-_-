using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public interface IAppointmentServicesService
    {
        Task<IReadOnlyList<AppointmentServiceItemDto>> GetAsync(long appointmentId, CancellationToken ct = default);
        Task<long> AddAsync(long appointmentId, int serviceId, int qty = 1, decimal? price = null, CancellationToken ct = default);
        Task UpdateAsync(AppointmentServiceItemDto dto, CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);
    }   
}

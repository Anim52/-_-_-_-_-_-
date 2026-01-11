using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public interface IAppointmentServiceItemService
    {
        Task<IReadOnlyList<AppointmentServiceItemDto>> GetAsync(long appointmentId);
        Task AddAsync(long appointmentId, int serviceId, int qty);
        Task UpdateAsync(long id, int qty, decimal price);
        Task DeleteAsync(long id);
    }
}

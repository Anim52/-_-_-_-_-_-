using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public interface IVisitService
    {
        Task<VisitDto> GetOrCreateByAppointmentAsync(long appointmentId, int doctorId, CancellationToken ct = default);
        Task SaveAsync(VisitDto dto, CancellationToken ct = default);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Services
{
    public interface IDoctorDayOffService
    {
        Task AddDayOffAsync(int doctorId, DateTime date, string? reason, CancellationToken ct = default);
        Task<bool> IsDayOffAsync(int doctorId, DateTime date, CancellationToken ct = default);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Patients
{
    public interface IPatientHistoryService
    {
        Task<IReadOnlyList<PatientHistoryItemDto>> GetAsync(int patientId, CancellationToken ct = default);
    }
}

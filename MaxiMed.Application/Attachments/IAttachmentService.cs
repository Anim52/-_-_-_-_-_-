using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Attachments
{
    public interface IAttachmentService
    {
        Task<IReadOnlyList<AttachmentDto>> GetByPatientAsync(int patientId, CancellationToken ct = default);
        Task<IReadOnlyList<AttachmentDto>> GetByVisitAsync(long visitId, CancellationToken ct = default);

        Task<long> UploadAsync(
            int patientId,
            long? visitId,
            string fileName,
            string? contentType,
            byte[] data,
            CancellationToken ct = default);

        Task<(string FileName, string? ContentType, byte[] Data)> DownloadAsync(long id, CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);
    }
}

using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class Attachment : Entity<long>
    {
        public int PatientId { get; set; }
        public long? VisitId { get; set; }

        public string FileName { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? StoragePath { get; set; }
        public byte[]? FileBlob { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public Visit? Visit { get; set; }
    }
}

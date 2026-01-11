using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Attachments
{
    public sealed class AttachmentDto
    {
        public long Id { get; set; }
        public int PatientId { get; set; }
        public long? VisitId { get; set; }

        public string FileName { get; set; } = "";
        public string? ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}

using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Attachments
{
    public sealed class AttachmentService : IAttachmentService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public AttachmentService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<AttachmentDto>> GetByPatientAsync(int patientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Attachments.AsNoTracking()
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.UploadedAt)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    VisitId = a.VisitId,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    UploadedAt = a.UploadedAt
                })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<AttachmentDto>> GetByVisitAsync(long visitId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Attachments.AsNoTracking()
                .Where(a => a.VisitId == visitId)
                .OrderByDescending(a => a.UploadedAt)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    VisitId = a.VisitId,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    UploadedAt = a.UploadedAt
                })
                .ToListAsync(ct);
        }

        public async Task<long> UploadAsync(
            int patientId,
            long? visitId,
            string fileName,
            string? contentType,
            byte[] data,
            CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = new Attachment
            {
                PatientId = patientId,
                VisitId = visitId,
                FileName = fileName,
                ContentType = contentType,
                FileBlob = data,
                UploadedAt = DateTime.UtcNow
            };

            db.Attachments.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task<(string FileName, string? ContentType, byte[] Data)> DownloadAsync(long id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var a = await db.Attachments.FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new InvalidOperationException("Файл не найден");

            return (a.FileName, a.ContentType, a.FileBlob ?? Array.Empty<byte>());
        }

        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var a = await db.Attachments.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (a is null) return;

            db.Attachments.Remove(a);
            await db.SaveChangesAsync(ct);
        }
    }
}

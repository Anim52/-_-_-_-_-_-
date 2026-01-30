using MaxiMed.Application.Attachments;

namespace MaxiMed.Wpf.Api;

public sealed class AttachmentsApiService : IAttachmentService
{
    private readonly ApiClient _api;
    public AttachmentsApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<AttachmentDto>> GetByPatientAsync(int patientId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<AttachmentDto>>($"api/attachments/by-patient/{patientId}", ct)) ?? Array.Empty<AttachmentDto>();

    public async Task<IReadOnlyList<AttachmentDto>> GetByVisitAsync(long visitId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<AttachmentDto>>($"api/attachments/by-visit/{visitId}", ct)) ?? Array.Empty<AttachmentDto>();

    private sealed record UploadRequest(int PatientId, long? VisitId, string FileName, string? ContentType, byte[] Data);

    public async Task<long> UploadAsync(int patientId, long? visitId, string fileName, string? contentType, byte[] data, CancellationToken ct = default)
        => (await _api.PostAsync<long>("api/attachments/upload", new UploadRequest(patientId, visitId, fileName, contentType, data), ct))!;

    private sealed record DownloadResponse(string FileName, string? ContentType, byte[] Data);

    public async Task<(string FileName, string? ContentType, byte[] Data)> DownloadAsync(long id, CancellationToken ct = default)
    {
        var res = (await _api.GetAsync<DownloadResponse>($"api/attachments/{id}/download", ct))!;
        return (res.FileName, res.ContentType, res.Data);
    }

    public Task DeleteAsync(long id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/attachments/{id}", ct);
}

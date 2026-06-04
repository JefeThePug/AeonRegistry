

namespace AeonRegistry.Services.Interfaces
{
    public interface IArtifactService
    {
        Task<List<PublicArtifactResponse>> GetPublicArtifactsAsync(CancellationToken ct);
        Task<PublicArtifactResponse?> GetPublicArtifactByIdAsync(int id, CancellationToken ct);
        Task<List<PublicArtifactResponse>> GetPublicArtifactsBySiteAsync(int siteId, CancellationToken ct);

        Task<List<PrivateArtifactResponse>> GetPrivateArtifactsAsync(CancellationToken ct);
        Task<PrivateArtifactResponse?> GetPrivateArtifactByIdAsync(int id, CancellationToken ct);
        Task<List<PrivateArtifactResponse>> GetPrivateArtifactsBySiteAsync(int siteId, CancellationToken ct);
        Task<PrivateArtifactResponse?> CreateArtifactAsync(CreateArtifactRequest request, CancellationToken ct);
        Task<bool> UpdateArtifactAsync(int id, UpdateArtifactRequest request, CancellationToken ct);
        Task<bool> DeleteArtifactAsync(int id, CancellationToken ct);
    }
}
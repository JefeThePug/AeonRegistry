
namespace AeonRegistry.Services.Interfaces
{
    public interface IArtifactMediaFileService
    {
        Task<ArtifactMediaFile?> GetArtifactMediaFileAsync(
            int id, CancellationToken ct);
        Task<ArtifactMediaFile?> CreateArtifactMediaFileAsync(
            int artifactId, IFormFile file, bool isPrimary, CancellationToken ct);
    }
}
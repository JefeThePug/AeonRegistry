
namespace AeonRegistry.Services.Interfaces
{
    public interface ISiteService
    {
        // READ
        Task<List<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct);
        Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int id, CancellationToken ct);
        Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct);
        Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct);
        // CREATE
        Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct);
        // UPDATE
        Task<bool> UpdateSiteAsync(int id, UpdateSiteRequest request, CancellationToken ct);
        // DELETE
        Task<bool> DeleteSiteAsync(int id, CancellationToken ct);
    }
}

namespace AeonRegistry.Services.Interfaces
{
    public interface ISiteService
    {
        Task<List<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct);
        Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int id, CancellationToken ct);
        Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct);
        Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct);
    }
}
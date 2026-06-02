
namespace AeonRegistry.Services.Interfaces
{
    public interface ISiteService
    {
        Task<List<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct);
    }
}
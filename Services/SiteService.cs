
using System.Linq.Expressions;
using AeonRegistry.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistry.Services
{
    public class SiteService(ApplicationDbContext db) : ISiteService
    {
        // Reusable Query Projections
        private static readonly Expression<Func<Site, PublicSiteResponse>> PublicSiteSelector =
            s => new PublicSiteResponse
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Coordinates = s.Coordinates,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Description = s.Description,
                PublicNarrative = s.PublicNarrative
            };
        private static readonly Expression<Func<Site, PrivateSiteResponse>> PrivateSiteSelector =
            s => new PrivateSiteResponse
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Coordinates = s.Coordinates,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Description = s.Description,
                PublicNarrative = s.PublicNarrative,
                AeonNarrative = s.AeonNarrative
            };

        // Services
        public async Task<List<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct)
        {
            return await db.Sites
                .AsNoTracking()
                .Select(PublicSiteSelector)
                .ToListAsync(ct);
        }

        public async Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int id, CancellationToken ct)
        {
            return await db.Sites
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(PublicSiteSelector)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct)
        {
            return await db.Sites
                .AsNoTracking()
                .Select(PrivateSiteSelector)
                .ToListAsync(ct);
        }

        public async Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct)
        {
            return await db.Sites
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(PrivateSiteSelector)
                .FirstOrDefaultAsync(ct);
        }
    }
}
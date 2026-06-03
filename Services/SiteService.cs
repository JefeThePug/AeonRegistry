
using System.Data;
using System.Formats.Cbor;
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

        public async Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct)
        {
            var site = new Site
            {
                Name = request.Name,
                Location = request.Location,
                Coordinates = request.Coordinates,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Description = request.Description,
                PublicNarrative = request.PublicNarrative,
                AeonNarrative = request.AeonNarrative
            };
            db.Sites.Add(site);
            await db.SaveChangesAsync(ct);
            return new PrivateSiteResponse
            {
                Id = site.Id,
                Name = site.Name,
                Location = site.Location,
                Coordinates = site.Coordinates,
                Latitude = site.Latitude,
                Longitude = site.Longitude,
                Description = site.Description,
                PublicNarrative = site.PublicNarrative,
                AeonNarrative = site.AeonNarrative
            };
        }

        public async Task<bool> UpdateSiteAsync(int id, UpdateSiteRequest request, CancellationToken ct)
        {
            var site = await db.Sites.FindAsync(id, ct);
            if (site is null)
            {
                return false;
            }
            site.Name = request.Name;
            site.Location = request.Location;
            site.Coordinates = request.Coordinates;
            site.Latitude = request.Latitude;
            site.Longitude = request.Longitude;
            site.Description = request.Description;
            site.PublicNarrative = request.PublicNarrative;
            site.AeonNarrative = request.AeonNarrative;

            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteSiteAsync(int id, CancellationToken ct)
        {
            var site = await db.Sites.FindAsync(id, ct);
            if (site is null)
            {
                return false;
            }
            db.Sites.Remove(site);
            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}
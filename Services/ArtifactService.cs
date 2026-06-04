using System.Data;
using System.Linq.Expressions;
using AeonRegistry.Endpoints.Artifact;
using AeonRegistry.Enums;
using AeonRegistry.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistry.Services
{
    public class ArtifactService(ApplicationDbContext db) : IArtifactService
    {
        private static readonly Expression<Func<Artifact, PublicArtifactResponse>> PublicArtifactSelector =
            a => new PublicArtifactResponse
            {
                Id = a.Id,
                Name = a.Name,
                CatalogNumber = a.CatalogNumber,
                PublicNarrative = a.PublicNarrative,
                DateDiscovered = a.DateDiscovered,
                Type = a.Type != null ? a.Type.ToString() : String.Empty,
                SiteId = a.SiteId,
                SiteName = a.Site != null ? a.Site.Name : String.Empty,
                PrimaryImageUrl = a.MediaFiles
                        .Where(m => m.IsPrimary)
                        .Select(m => $"/api/public/artifacts/images/{m.Id}")
                        .FirstOrDefault()
            };

        private static readonly Expression<Func<Artifact, PrivateArtifactResponse>> PrivateArtifactSelector =
            a => new PrivateArtifactResponse
            {
                Id = a.Id,
                Name = a.Name,
                CatalogNumber = a.CatalogNumber,
                PublicNarrative = a.PublicNarrative,
                Description = a.Description,
                DateDiscovered = a.DateDiscovered,
                Type = a.Type != null ? a.Type.ToString() : String.Empty,
                SiteId = a.SiteId,
                SiteName = a.Site != null ? a.Site.Name : String.Empty,
                PrimaryImageUrl = a.MediaFiles
                        .Where(m => m.IsPrimary)
                        .Select(m => $"/api/public/artifacts/images/{m.Id}")
                        .FirstOrDefault()
            };


        public async Task<List<PublicArtifactResponse>> GetPublicArtifactsAsync(CancellationToken ct)
        {
            return await db.Artifacts
                .AsNoTracking()
                .Select(PublicArtifactSelector)
                .ToListAsync(ct);
        }

        public async Task<PublicArtifactResponse?> GetPublicArtifactByIdAsync(int id, CancellationToken ct)
        {
            return await db.Artifacts
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(PublicArtifactSelector)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<PublicArtifactResponse>> GetPublicArtifactsBySiteAsync(int siteId, CancellationToken ct)
        {
            var siteExists = await db.Sites.AsNoTracking().AnyAsync(s => s.Id == siteId, ct);
            if (!siteExists)
            {
                return [];
            }
            return await db.Artifacts
                .AsNoTracking()
                .Where(a => a.SiteId == siteId)
                .Select(PublicArtifactSelector)
                .ToListAsync(ct);
        }


        public async Task<List<PrivateArtifactResponse>> GetPrivateArtifactsAsync(CancellationToken ct)
        {
            return await db.Artifacts
                .AsNoTracking()
                .Select(PrivateArtifactSelector)
                .ToListAsync(ct);
        }

        public async Task<PrivateArtifactResponse?> GetPrivateArtifactByIdAsync(int id, CancellationToken ct)
        {
            return await db.Artifacts
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(PrivateArtifactSelector)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<PrivateArtifactResponse>> GetPrivateArtifactsBySiteAsync(int siteId, CancellationToken ct)
        {
            var siteExists = await db.Sites.AsNoTracking().AnyAsync(s => s.Id == siteId, ct);
            if (!siteExists)
            {
                return [];
            }
            return await db.Artifacts
                .AsNoTracking()
                .Where(a => a.SiteId == siteId)
                .Select(PrivateArtifactSelector)
                .ToListAsync(ct);
        }

        public async Task<PrivateArtifactResponse?> CreateArtifactAsync(CreateArtifactRequest request, CancellationToken ct)
        {
            var site = await db.Sites.FindAsync(request.SiteId, ct);
            if (site == null)
            {
                return null;
            }
            if (!Enum.TryParse<ArtifactType>(request.Type, true, out var artifactType))
            {
                throw new ArgumentException("Invalid Artifact Type.");
            }

            var artifact = new Artifact
            {
                Name = request.Name,
                CatalogNumber = request.CatalogNumber,
                Description = request.Description,
                PublicNarrative = request.PublicNarrative,
                Type = artifactType.ToString(), // or request.Type
                SiteId = request.SiteId,
                DateDiscovered = request.DateDiscovered
            };
            db.Artifacts.Add(artifact);
            await db.SaveChangesAsync(ct);

            return new PrivateArtifactResponse
            {
                Id = artifact.Id,
                Name = artifact.Name,
                CatalogNumber = artifact.CatalogNumber,
                Description = artifact.Description,
                PublicNarrative = artifact.PublicNarrative,
                DateDiscovered = artifact.DateDiscovered,
                Type = artifact.Type,
                SiteId = site.Id,
                SiteName = site.Name,
                PrimaryImageUrl = "" // Image added later with the other API endpoint
            };
        }

        public async Task<bool> UpdateArtifactAsync(int id, UpdateArtifactRequest request, CancellationToken ct)
        {
            var siteExists = await db.Sites.AsNoTracking().AnyAsync(s => s.Id == request.SiteId);
            if (!siteExists)
            { return false; }

            var artifact = await db.Artifacts.FindAsync(id, ct);
            if (artifact is null)
            { return false; }

            if (!Enum.TryParse<ArtifactType>(request.Type, true, out var artifactType))
            { throw new ArgumentException("Invalid Artifact Type."); }

            artifact.Name = request.Name;
            artifact.CatalogNumber = request.CatalogNumber;
            artifact.Description = request.Description;
            artifact.PublicNarrative = request.PublicNarrative;
            artifact.Type = request.Type;
            artifact.SiteId = request.SiteId;
            artifact.DateDiscovered = request.DateDiscovered;

            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteArtifactAsync(int id, CancellationToken ct)
        {
            var artifact = await db.Artifacts.FirstOrDefaultAsync(a => a.Id == id, ct);

            if (artifact is null)
            { return false; }

            // Cascade Deletes here
            if (artifact.MediaFiles != null && artifact.MediaFiles.Count > 0)
            {
                db.ArtifactMediaFiles.RemoveRange(artifact.MediaFiles);
            }

            db.Artifacts.Remove(artifact);
            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}
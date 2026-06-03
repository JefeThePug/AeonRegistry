using AeonRegistry.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistry.Services
{
    public class ArtifactMediaFileService(ApplicationDbContext db) : IArtifactMediaFileService
    {
        public async Task<ArtifactMediaFile?> GetArtifactMediaFileAsync(int id, CancellationToken ct)
        {
            return await db.ArtifactMediaFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<ArtifactMediaFile?> CreateArtifactMediaFileAsync(
            int artifactId, IFormFile file, bool isPrimary, CancellationToken ct)
        {
            var artifact = await db.Artifacts.FindAsync(artifactId, ct);
            if (artifact is null)
            {
                return null;
            }

            // Validate input
            if (isPrimary)
            {
                var existingPrimary = await db.ArtifactMediaFiles
                    .Where(m => m.ArtifactId == artifactId && m.IsPrimary == true)
                    .ToListAsync(ct);
                foreach (var mediaFile in existingPrimary)
                {
                    mediaFile.IsPrimary = false;
                }
            }

            // Convert IFormFile to byte[]
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var data = ms.ToArray();

            // Create media file
            var newMedia = new ArtifactMediaFile
            {
                ArtifactId = artifactId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = data,
                IsPrimary = isPrimary
            };

            db.ArtifactMediaFiles.Add(newMedia);
            await db.SaveChangesAsync(ct);
            return newMedia;
        }
    }
}
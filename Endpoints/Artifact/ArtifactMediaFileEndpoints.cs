
using AeonRegistry.Filters;
using AeonRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistry.Endpoints.Artifact
{
    public static class ArtifactMediaFileEndpoints
    {
        // Groups
        public static IEndpointRouteBuilder MapArtifactMediaFileEndpoints(this IEndpointRouteBuilder route)
        {
            var group = route.MapGroup("/api/public/artifacts/images")
                .AllowAnonymous()
                .WithSummary("Artifact Image Endpoints")
                .WithDescription("Endpoints for artifact images")
                .WithTags("Artifact Media Files")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            group.MapGet("/{id:int}", GetArtifactImage)
                .WithName(nameof(GetArtifactImage))
                .WithSummary("Get Artifact Image")
                .WithDescription("Returns the binary image data for an artifact")
                .Produces<FileContentHttpResult>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            var privateGroup = route.MapGroup("/api/private/artifacts/{artifactId:int}/images")
                .RequireAuthorization()
                .WithSummary("Private Artifact Image Endpoints")
                .WithDescription("Private endpoints for artifact images (requiring authorization)")
                .WithTags("Artifact Media Files (Private)")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            privateGroup.MapPost("", CreateArtifactMediaFile)
                .DisableAntiforgery() // API doesn't need this
                .WithName(nameof(CreateArtifactMediaFile))
                .WithSummary("Upload an Artifact Image (Private)")
                .WithDescription("Upload a new image file and associates it with an artifact (Private)")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            return route;
        }

        // Handlers
        private static async Task<Results<FileContentHttpResult, NotFound>> GetArtifactImage(
            int id,
            IArtifactMediaFileService service,
            HttpResponse response,
            CancellationToken ct)
        {
            var image = await service.GetArtifactMediaFileAsync(id, ct);
            if (image is null || image.Data.Length == 0)
            {
                return TypedResults.NotFound();
            }

            // optional: Cache frequently requested images for faster loading
            response.Headers.CacheControl = "public,max-age=604800";

            return TypedResults.File(image.Data, image.ContentType);
        }

        private static async Task<Results<Created, NotFound, BadRequest>> CreateArtifactMediaFile(
            int artifactId,
            IFormFile file,
            bool isPrimary,
            IArtifactMediaFileService service,
            CancellationToken ct)
        {
            if (file is null || file.Length == 0)
            {
                return TypedResults.BadRequest();
            }
            var media = await service.CreateArtifactMediaFileAsync(artifactId, file, isPrimary, ct);
            if (media is null)
            {
                return TypedResults.NotFound();
            }
            var location = $"/api/public/artifacts/images/{media.Id}";
            return TypedResults.Created(location);
        }
    }
}
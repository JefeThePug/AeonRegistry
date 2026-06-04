
using AeonRegistry.Filters;
using AeonRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AeonRegistry.Endpoints.Artifact
{
    public static class ArtifactEndpoints
    {
        public static IEndpointRouteBuilder MapArtifactEndpoints(this IEndpointRouteBuilder route)
        {
            var publicGroup = route.MapGroup("/api/public/artifacts")
                .AllowAnonymous()
                .WithSummary("Public Artifact Endpoints")
                .WithDescription("Endpoints for artifacts")
                .WithTags("Artifacts (Public)")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            publicGroup.MapGet("", GetPublicArtifacts)
                .WithName(nameof(GetPublicArtifacts))
                .WithSummary("Get All Artifacts (Public Data)")
                .WithDescription("Returns the public data for all artifacts")
                .Produces<List<PublicArtifactResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            publicGroup.MapGet("/{id:int}", GetPublicArtifactById)
                .WithName(nameof(GetPublicArtifactById))
                .WithSummary("Get Artifact By ID (Public Data)")
                .WithDescription("Returns the public data for a selected artifact")
                .Produces<PublicArtifactResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            var privateGroup = route.MapGroup("/api/private/artifacts")
                .RequireAuthorization()
                .WithSummary("Artifact Endpoints (Private)")
                .WithDescription("Endpoints for artifacts")
                .WithTags("Artifacts (Private)")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            privateGroup.MapGet("", GetPrivateArtifacts)
                .WithName(nameof(GetPrivateArtifacts))
                .WithSummary("Get All Artifacts (Private Data)")
                .WithDescription("Returns all the data for all artifacts")
                .Produces<List<PrivateArtifactResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapGet("/{id:int}", GetPrivateArtifactById)
                .WithName(nameof(GetPrivateArtifactById))
                .WithSummary("Get Artifact By ID (Private Data)")
                .WithDescription("Returns all the data for a chosen artifact")
                .Produces<PrivateArtifactResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapPost("", CreateArtifact)
                .WithName(nameof(CreateArtifact))
                .WithSummary("Create New Artifact Record")
                .WithDescription("Creates a new artifact record with description")
                .Produces<PrivateArtifactResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .ProducesValidationProblem();

            privateGroup.MapPut("/{id:int}", UpdateArtifact)
                .WithName(nameof(UpdateArtifact))
                .WithSummary("Update Artifact")
                .WithDescription("Updates an artifact record")
                .Produces<PrivateArtifactResponse>(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .ProducesValidationProblem();

            privateGroup.MapDelete("/{id:int}", DeleteArtifact)
                .WithName(nameof(DeleteArtifact))
                .WithSummary("Delete Artifact")
                .WithDescription("Delete an artifact record")
                .Produces<PrivateArtifactResponse>(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            return route;
        }

        private static async Task<Results<Ok<List<PublicArtifactResponse>>, NotFound>> GetPublicArtifacts(
            IArtifactService service,
            CancellationToken cf)
        {
            var artifacts = await service.GetPublicArtifactsAsync(cf);
            if (artifacts is null || artifacts.Count == 0)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(artifacts);
        }

        private static async Task<Results<Ok<PublicArtifactResponse>, NotFound>> GetPublicArtifactById(
            int id,
            IArtifactService service,
            CancellationToken ct)
        {
            var artifact = await service.GetPublicArtifactByIdAsync(id, ct);
            return artifact is null ? TypedResults.NotFound() : TypedResults.Ok(artifact);
        }

        private static async Task<Results<Ok<List<PrivateArtifactResponse>>, NotFound>> GetPrivateArtifacts(
            IArtifactService service,
            CancellationToken cf)
        {
            var artifacts = await service.GetPrivateArtifactsAsync(cf);
            if (artifacts is null || artifacts.Count == 0)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(artifacts);
        }

        private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> GetPrivateArtifactById(
            int id,
            IArtifactService service,
            CancellationToken ct)
        {
            var artifact = await service.GetPrivateArtifactByIdAsync(id, ct);
            return artifact is null ? TypedResults.NotFound() : TypedResults.Ok(artifact);
        }

        private static async Task<Results<Created<PrivateArtifactResponse>, NotFound>> CreateArtifact(
            IArtifactService service,
            CreateArtifactRequest request,
            CancellationToken ct)
        {
            var created = await service.CreateArtifactAsync(request, ct);
            if (created is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Created($"/api/private/artifacts/{created.Id}", created);
        }

        private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateArtifact(
            int id,
            IArtifactService service,
            UpdateArtifactRequest request,
            CancellationToken ct)
        {
            var success = await service.UpdateArtifactAsync(id, request, ct);
            return success ? TypedResults.NoContent() : TypedResults.NotFound();
        }

        private static async Task<Results<NoContent, NotFound>> DeleteArtifact(
            int id,
            IArtifactService service,
            CancellationToken ct)
        {
            var success = await service.DeleteArtifactAsync(id, ct);
            return success ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
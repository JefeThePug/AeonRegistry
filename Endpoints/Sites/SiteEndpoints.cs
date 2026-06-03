
using System.ComponentModel;
using AeonRegistry.Filters;
using AeonRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AeonRegistry.Endpoints.Sites
{
    public static class SiteEndpoints
    {
        // Endpoint Groups for Sites
        public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder route)
        {
            var publicGroup = route.MapGroup("/api/public/sites")
                .AllowAnonymous()
                .WithSummary("Public Site Endpoints")
                .WithDescription("Endpoints that expose public site data")
                .WithTags("Sites - Public")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            publicGroup.MapGet("", GetAllPublicSites)
                .WithName(nameof(GetAllPublicSites))
                .WithSummary("Get All Sites (Public)")
                .WithDescription("Returns all sites with their public data only")
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError);

            publicGroup.MapGet("{id:int}", GetPublicSiteById)
                .WithName(nameof(GetPublicSiteById))
                .WithSummary("Get Site By ID (Public)")
                .WithDescription("Returns the public data (only) from the site which matches the user-provided ID")
                .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            var privateGroup = route.MapGroup("/api/private/sites")
                .RequireAuthorization()
                .WithSummary("Private Site Endpoints")
                .WithDescription("Endpoints that require authentication")
                .WithTags("Sites - Private")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            privateGroup.MapGet("", GetAllPrivateSites)
                .WithName(nameof(GetAllPrivateSites))
                .WithSummary("Get All Sites (Private)")
                .WithDescription("Returns all sites with their private data")
                .Produces<List<PrivateSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapGet("{id:int}", GetPrivateSiteById)
                .WithName(nameof(GetPrivateSiteById))
                .WithSummary("Get Site By ID (Private)")
                .WithDescription("Returns all data from the site which matches the user-provided ID")
                .Produces<PrivateSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapPost("", CreateSite)
                .WithName(nameof(CreateSite))
                .WithSummary("Create New Site (Private)")
                .WithDescription("Create a site - Requires Authentication")
                .Accepts<CreateSiteRequest>("application/json")
                .Produces<PrivateSiteResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError)
                .ProducesValidationProblem();

            privateGroup.MapPut("/{id:int}", UpdateSite)
                .WithName(nameof(UpdateSite))
                .WithSummary("Update a Site (Private)")
                .WithDescription("Update an existing site - Requires Authentication")
                .Accepts<UpdateSiteRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .ProducesValidationProblem();

            privateGroup.MapDelete("/{id:int}", DeleteSite)
                .WithName(nameof(DeleteSite))
                .WithSummary("Delete a Site (Private)")
                .WithDescription("Delete an existing site - Requires Authentication")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            return route;
        }

        // Method Handlers for Sites
        private static async Task<Ok<List<PublicSiteResponse>>> GetAllPublicSites(
            ISiteService service,
            CancellationToken ct)
        {
            return TypedResults.Ok(await service.GetAllPublicSitesAsync(ct));
        }

        private static async Task<Results<Ok<PublicSiteResponse>, NotFound>> GetPublicSiteById(
            int id,
            ISiteService service,
            CancellationToken ct)
        {
            var site = await service.GetPublicSiteByIdAsync(id, ct);
            if (site is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(site);
        }

        private static async Task<Ok<List<PrivateSiteResponse>>> GetAllPrivateSites(
            ISiteService service,
            CancellationToken ct)
        {
            return TypedResults.Ok(await service.GetAllPrivateSitesAsync(ct));
        }

        private static async Task<Results<Ok<PrivateSiteResponse>, NotFound>> GetPrivateSiteById(
            int id,
            ISiteService service,
            CancellationToken ct)
        {
            var site = await service.GetPrivateSiteByIdAsync(id, ct);
            if (site is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(site);
        }

        private static async Task<Results<Created<PrivateSiteResponse>, ValidationProblem>> CreateSite(
            CreateSiteRequest request,
            ISiteService service,
            CancellationToken ct)
        {
            var createdSite = await service.CreateSiteAsync(request, ct);
            return TypedResults.Created($"/api/private/sites/{createdSite.Id}", createdSite);
        }

        private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateSite(
            int id,
            UpdateSiteRequest request,
            ISiteService service,
            CancellationToken ct)
        {
            var success = await service.UpdateSiteAsync(id, request, ct);
            return success ? TypedResults.NoContent() : TypedResults.NotFound();
        }

        private static async Task<Results<NoContent, NotFound>> DeleteSite(
            int id,
            ISiteService service,
            CancellationToken ct)
        {
            var success = await service.DeleteSiteAsync(id, ct);
            return success ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
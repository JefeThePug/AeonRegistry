
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
                .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
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
    }
}

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
                .WithTags("Sites - Public");

            publicGroup.MapGet("", GetAllPublicSites)
                .WithName(nameof(GetAllPublicSites))
                .WithSummary("Get All Sites (Public)")
                .WithDescription("Returns all sites with their public data only")
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK);

            return route;
        }

        // Method Handlers for Sites
        private static async Task<Ok<List<PublicSiteResponse>>> GetAllPublicSites(
            ISiteService service,
            CancellationToken ct)
        {
            return TypedResults.Ok(await service.GetAllPublicSitesAsync(ct));
        }

    }
}
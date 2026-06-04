
using AeonRegistry.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AeonRegistry.Extensions;

public static class OpenAPIExtensions
{
    public static IServiceCollection AddOpenAPIDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Aeon Registry API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Aeon Registry Team",
                    Url = new Uri("https://github.com/JefeThePug"),
                    Email = "jefethepug@protonmail.com"
                },
                Description = """
                ![Aeon Logo](/images/AeonRegistryLogo.png)

                ## Aeon Research Division

                Internal API for managing recovered artifacts and research data.
                Provides secure access for field researchers and analysts.

                ---

                ### Key Features
                - Site and Artifact Catalog
                - Research Record Submissions
                - Secure Media Storage
                - User Role Management
                
                [Launch Public Test Site](/site/sites-map.html)
                
                """
            });
            c.UseInlineDefinitionsForEnums();
            c.SchemaFilter<EnumStringSchemaFilter>();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid JWT token."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            string[] hiddenEndpoints = [
                "api/auth/register",
                "api/auth/refresh",
                "api/auth/confirmemail",
                "api/auth/resendconfirmationemail",
                "api/auth/forgotpassword",
                "api/auth/resetpassword",
                "api/auth/manage",
                "api/auth/manage/info",
                "api/auth/manage/2fa",
            ];

            c.DocInclusionPredicate((docName, ApiDescription) =>
            {
                var path = ApiDescription.RelativePath?.ToLowerInvariant();
                if (path is null)
                    return false;
                if (hiddenEndpoints.Contains(path, StringComparer.OrdinalIgnoreCase))
                    return false;
                return true;
            });

            c.MapType<DateTime>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date-time",
                Example = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
            });
        });

        return services;
    }
}

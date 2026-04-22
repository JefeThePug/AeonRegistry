using System.Transactions;
using Microsoft.OpenApi.Models;

namespace AeonRegistry.Extensions;

public static class OpenAPISwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
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
                <img src="/images/AeonRegistryLogoBLK.png" height="120" />

                ## Aeon Research Division

                Internal API for managing recovered artifacts and research data.
                Provides secure access for field researchers and analysts.

                ### Key Features
                - Site and Artifact Catalog
                - Research Record Submissions
                - Secure Media Storage
                - User Role Management

                ---
                <small>
                AeonRegistry ©2026
                </small>
                """
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your vaili JWT token."
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
        });

        return services;
    }
}


using AeonRegistry.Endpoints.Home;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenAPIDocumentation();
// get connection to the database
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

// configure database context for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Aeon Registry API";
        options.Theme = ScalarTheme.Kepler;
        options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// map Home Group endpoints
app.MapHomeEndpoints();

app.Run();

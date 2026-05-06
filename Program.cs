
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenAPIDocumentation();

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


app.MapGet("/api/Welcome", () =>
{
    var response = new
    {
        Message = "Welcome to the Aeon Registry API",
        Version = "1.0.0",
        TimeOnly = DateTime.Now.ToString("T")
    };
    return Results.Ok(response);
}).WithName("WelcomeMessage");

app.Run();

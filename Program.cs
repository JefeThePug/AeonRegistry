
using AeonRegistry.Endpoints.CustomIdentityEndpoints;
using AeonRegistry.Endpoints.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

// Builder Stage

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenAPIDocumentation();
// get connection to the database
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

// configure database context for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// add identity endpoints
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// admin policy
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

// email sender services
builder.Services.AddTransient<IEmailSender, ConsoleEmailService>();

// enable validation for minimal APIs
builder.Services.AddValidation();

var app = builder.Build();

// App Stage
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

using (var scope = app.Services.CreateScope())
{
    await DataSeed.ManageDataAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BlockIdentityEndpoints>();

var authRouteGroup = app.MapGroup("/api/auth")
    .WithTags("Admin");
authRouteGroup.MapIdentityApi<ApplicationUser>();


app.MapHomeEndpoints();
app.MapCustomIdentityEndpoints();

app.Run();

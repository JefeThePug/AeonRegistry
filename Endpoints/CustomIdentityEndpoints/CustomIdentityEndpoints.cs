
using AeonRegistry.Endpoints.CustomIdentityEndpoints.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace AeonRegistry.Endpoints.CustomIdentityEndpoints;

public static class CustomIdentityEndpoints
{
    public static IEndpointRouteBuilder MapCustomIdentityEndpoints(this IEndpointRouteBuilder route)
    {
        // Make Group
        var group = route.MapGroup("/api/auth")
            .WithTags("Admin");

        // Make Endpoints
        group.MapPost("/register-admin", RegisterUser)
            .WithName("RegisterAdmin")
            .WithSummary("Register User")
            .WithDescription("Registers a user must have admin role")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        //.RequireAuthorization("AdminPolicy");

        group.MapPost("/reset-password", ResetPassword)
            .WithName("ResetPassword")
            .WithSummary("Reset User Password")
            .WithDescription("Custom Reset Password for a user")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/forgot-password", ForgotPassword)
            .WithName("ForgotPassword")
            .WithSummary("Custom Forgot Password")
            .WithDescription("Custom Forgot Password flow")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/manage/profile", GetProfileInfo)
            .WithName("GetProfileInfo")
            .WithSummary("Get Current User Profile")
            .WithDescription("Get current user profile info")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();


        // Return Route
        return route;
    }

    private static async Task<IResult> RegisterUser(
        RegisterUserRequest request,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEmailSender emailSender,
        IConfiguration config)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Results.BadRequest(new { Error = $"User with email {request.Email} already exists" });
        }
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var tempPassword = "TempPassword123!";

        var created = await userManager.CreateAsync(user, tempPassword);

        if (!created.Succeeded)
        {
            return Results.BadRequest(new { Error = created.Errors });
        }

        if (await roleManager.RoleExistsAsync("Researcher"))
        {
            await userManager.AddToRoleAsync(user, "Researcher");
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var baseURL = config["BaseURL"] ?? "https://localhost:5252";
        var resetLink = $"{baseURL}/SetPassword.html?email={request.Email}&resetCode={encodedToken}";

        await emailSender.SendEmailAsync(
            request.Email,
            "Welcome to Aeon",
            $"""
            Your account has been created. Please change your password by visiting:
            
            {resetLink}

            """
        );

        return Results.Ok(new { Message = $"User {user.Email} created. Password reset link sent." });
    }

    private static async Task<IResult> ResetPassword(
        ResetPasswordRequest request,
        UserManager<ApplicationUser> userManager)
    {
        if (
            string.IsNullOrEmpty(request.Email) ||
            string.IsNullOrEmpty(request.ResetCode) ||
            string.IsNullOrEmpty(request.NewPassword))
        {
            return Results.BadRequest(new { Message = "All Fields are required" });
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Results.BadRequest(new { Message = $"User {request.Email} not found" });
        }

        try
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
            var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
            if (!result.Succeeded)
            {
                return Results.BadRequest(new { Message = "Something went wrong." });
            }
        }
        catch (FormatException)
        {
            return Results.BadRequest(new { Message = "Invalid Token" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Message = $"Error: {ex.Message}" });
        }

        return Results.Ok(new { Message = "Password reset successfully" });
    }

    private static async Task<IResult> ForgotPassword(
        ForgotPasswordRequest request,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IConfiguration config)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return Results.BadRequest(new { Message = "Email Address is required" });
        }
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is not null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var baseURL = config["BaseURL"] ?? "https://localhost:5252";
            var resetLink = $"{baseURL}/SetPassword.html?email={request.Email}&resetCode={encodedToken}";

            await emailSender.SendEmailAsync(
                request.Email,
                "Reset Your Password",
                $"""
            To Reset your password, use the link:
            
            {resetLink}

            """
            );
        }

        return Results.Ok(new { Message = "If the email exists, a Forgot Password link will be sent" });
    }

    private static async Task<IResult> GetProfileInfo(
        ClaimsPrincipal principal,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return Results.NotFound();
        }

        var response = new UserProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            FullName = user.FullName
        };
        return Results.Ok(response);
    }
}

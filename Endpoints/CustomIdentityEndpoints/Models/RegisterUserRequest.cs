
namespace AeonRegistry.Endpoints.CustomIdentityEndpoints.Models;

public class RegisterUserRequest
{
    [Required]
    public required string Email { get; init; }
    [Required]
    public required string? FirstName { get; init; }
    [Required]
    public required string? LastName { get; init; }
}

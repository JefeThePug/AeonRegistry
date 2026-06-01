
using Microsoft.VisualBasic;

namespace AeonRegistry.Endpoints.CustomIdentityEndpoints.Models
{
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
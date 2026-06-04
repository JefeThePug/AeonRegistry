
using System.Text.Json.Serialization;

namespace AeonRegistry.Models.Response
{
    public class PrivateArtifactResponse : PublicArtifactResponse
    {
        [JsonPropertyOrder(10)]
        public string? Description { get; set; }
    }
}
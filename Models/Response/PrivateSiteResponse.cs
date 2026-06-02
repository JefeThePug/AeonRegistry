
using System.Text.Json.Serialization;

namespace AeonRegistry.Models.Response
{
    public class PrivateSiteResponse : PublicSiteResponse
    {
        [JsonPropertyOrder(10)] // Just for a pretty JSON with this field last
        public string? AeonNarrative { get; set; }
    }
}

namespace AeonRegistry.Models.Request
{
    public class UpdateArtifactRequest
    {
        [Required, MaxLength(200)]
        public string? Name { get; set; }
        [Required, MaxLength(50)]
        public string? CatalogNumber { get; set; }
        [Required, MaxLength(2000)]
        public string? Description { get; set; }
        [Required, MaxLength(2000)]
        public string? PublicNarrative { get; set; }
        public DateTime DateDiscovered { get; set; } = DateTime.UtcNow;
        [Required]
        public string? Type { get; set; }
        [Required]
        public int SiteId { get; set; }
    }
}
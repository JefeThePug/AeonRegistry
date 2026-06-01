
namespace AeonRegistry.Models
{
    public class CatalogRecord
    {
        public int Id { get; set; }
        [Required, MaxLength(500)]
        public int ArtifactId { get; set; }
        public Artifact? Artifact { get; set; } = null!;

        [Required]
        public string? SubmittedById { get; set; } = string.Empty; //fKey ApplicationUser
        public ApplicationUser? SubmittedBy { get; set; } = null!;

        public string? VerifiedById { get; set; } = string.Empty;
        public ApplicationUser? VerifiedBy { get; set; } = null!;

        public string Status { get; set; } = Enums.CatalogStatus.Draft.ToString();
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        public ICollection<CatalogNote> Notes { get; set; } = [];
    }
}
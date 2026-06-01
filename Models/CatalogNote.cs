
namespace AeonRegistry.Models
{
    public class CatalogNote
    {
        public int Id { get; set; }
        public int CatalogRecordId { get; set; }
        public CatalogRecord? CatalogRecord { get; set; } = null!;
        public string AuthorId { get; set; } = String.Empty;
        public ApplicationUser? Author { get; set; } = null!;
        [Required, MaxLength(1000)]
        public string? Content { get; set; } = string.Empty;
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
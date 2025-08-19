using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Metadados de um arquivo salvo no Azure Blob Storage
    /// </summary>
    public class Arquivo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string NomeOriginal { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public string BlobUrl { get; set; } = string.Empty;

        [Required]
        public string BlobName { get; set; } = string.Empty;

        [Required]
        public long TamanhoBytes { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Representa um anexo (arquivo) associado a uma proposta de negociação
    /// </summary>
    public class Anexo
    {
        public int Id { get; set; }

        /// <summary>
        /// Proposta à qual este anexo pertence
        /// </summary>
        [Required]
        public int PropostaNegociacaoId { get; set; }

        [ForeignKey("PropostaNegociacaoId")]
        public PropostaNegociacao? Proposta { get; set; }

        [Required]
        [StringLength(255)]
        public string NomeArquivo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public string BlobUrl { get; set; } = string.Empty;

        [Required]
        public string BlobName { get; set; } = string.Empty;

        [Required]
        [Range(1, long.MaxValue)]
        public long TamanhoBytes { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}

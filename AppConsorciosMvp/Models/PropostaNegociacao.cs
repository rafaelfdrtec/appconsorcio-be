using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppConsorciosMvp.Models.Enums;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Proposta de negociação entre comprador e vendedor para uma carta
    /// </summary>
    public class PropostaNegociacao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A carta de consórcio é obrigatória")]
        public int CartaConsorcioId { get; set; }

        [ForeignKey(nameof(CartaConsorcioId))]
        public CartaConsorcio? Carta { get; set; }

        [Required(ErrorMessage = "O comprador é obrigatório")]
        public int CompradorId { get; set; }

        [ForeignKey(nameof(CompradorId))]
        public Usuario? Comprador { get; set; }

        [Required(ErrorMessage = "O ágio é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O ágio deve ser maior ou igual a zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Agio { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O prazo em meses deve ser maior que zero")]
        public int? PrazoMeses { get; set; }

        public PropostaStatus Status { get; set; } = PropostaStatus.Iniciada;

        public string? MotivoCancelamento { get; set; }

        public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
        public DateTime? CanceladaEm { get; set; }
        public DateTime? EfetivadaEm { get; set; }

        // Metadados opcionais de anexo simples
        [StringLength(255)]
        public string? AnexoNomeArquivo { get; set; }

        [StringLength(100)]
        public string? AnexoContentType { get; set; }

        public string? AnexoBlobUrl { get; set; }
        public string? AnexoBlobName { get; set; }
        public long? AnexoTamanhoBytes { get; set; }

        /// <summary>
        /// Anexos vinculados a esta proposta
        /// </summary>
        public ICollection<PropostaAnexo>? Anexos { get; set; }
    }
}

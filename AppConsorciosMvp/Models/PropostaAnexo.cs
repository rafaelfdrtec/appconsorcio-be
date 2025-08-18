using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Vínculo entre uma proposta de negociação e um arquivo (anexo)
    /// </summary>
    public class PropostaAnexo
    {
        public Guid Id { get; set; }

        [Required]
        public int PropostaNegociacaoId { get; set; }

        [ForeignKey(nameof(PropostaNegociacaoId))]
        public PropostaNegociacao? Proposta { get; set; }

        [Required]
        public Guid ArquivoId { get; set; }

        [ForeignKey(nameof(ArquivoId))]
        public Arquivo? Arquivo { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}

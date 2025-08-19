using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Vínculo entre uma carta de consórcio e um arquivo (anexo)
    /// </summary>
    public class CartaAnexo
    {
        public int Id { get; set; }

        [Required]
        public int CartaConsorcioId { get; set; }

        [ForeignKey(nameof(CartaConsorcioId))]
        public CartaConsorcio? Carta { get; set; }

        [Required]
        public int ArquivoId { get; set; }

        [ForeignKey(nameof(ArquivoId))]
        public Arquivo? Arquivo { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}

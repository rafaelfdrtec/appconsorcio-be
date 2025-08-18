using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Parâmetros de configuração armazenados no banco
    /// </summary>
    public class ParametroSistema
    {
        /// <summary>
        /// Chave do parâmetro (PK)
        /// </summary>
        [Key]
        [StringLength(100)]
        public string Chave { get; set; } = string.Empty;

        /// <summary>
        /// Valor do parâmetro
        /// </summary>
        public string Valor { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional
        /// </summary>
        public string? Descricao { get; set; }
    }
}

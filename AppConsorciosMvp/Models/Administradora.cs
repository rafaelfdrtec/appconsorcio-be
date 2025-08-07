using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Representa uma administradora de consórcio no sistema
    /// </summary>
    public class Administradora
    {
        /// <summary>
        /// Identificador único da administradora
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome da administradora
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da administradora
        /// </summary>
        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
        public string Cnpj { get; set; } = string.Empty;

        /// <summary>
        /// Telefone da administradora
        /// </summary>
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }

        /// <summary>
        /// Email da administradora
        /// </summary>
        [EmailAddress(ErrorMessage = "Email deve estar em formato válido")]
        [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
        public string? Email { get; set; }

        /// <summary>
        /// Status da administradora (ativa ou inativa)
        /// </summary>
        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(10, ErrorMessage = "Status deve ter no máximo 10 caracteres")]
        public string Status { get; set; } = "ativa";

        /// <summary>
        /// Data de criação do registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Lista de cartas de consórcio desta administradora
        /// </summary>
        public ICollection<CartaConsorcio>? CartasConsorcio { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using AppConsorciosMvp.Models.Enums;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Representa um usuário do sistema
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único do usuário
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário (usado para login)
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email em formato inválido")]
        [StringLength(100, ErrorMessage = "O email não pode exceder 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash da senha do usuário
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória")]
        public string SenhaHash { get; set; } = string.Empty;

        /// <summary>
        /// Papel do usuário no sistema (admin, vendedor, comprador)
        /// </summary>
        [Required(ErrorMessage = "O papel do usuário é obrigatório")]
        public UsuarioPapel Papel { get; set; } = UsuarioPapel.Comprador;  // Valor padrão

        /// <summary>
        /// Indica se o usuário foi verificado pela equipe administrativa
        /// </summary>
        public bool EhVerificado { get; set; } = false;  // Valor padrão

        /// <summary>
        /// Nível de KYC do usuário (0=sem KYC)
        /// </summary>
        public int KycLevel { get; set; } = 0;

        /// <summary>
        /// Indica se o usuário possui MFA habilitado
        /// </summary>
        public bool MfaEnabled { get; set; } = false;

        /// <summary>
        /// Status do usuário (ex: active/suspended)
        /// </summary>
        [StringLength(30)]
        public string Status { get; set; } = "active";

        /// <summary>
        /// Lista de cartas de consórcio associadas a este usuário (como vendedor)
        /// </summary>
        public ICollection<CartaConsorcio>? CartasConsorcio { get; set; }

        /// <summary>
        /// Propostas de negociação em que este usuário participa como comprador
        /// </summary>
        public ICollection<PropostaNegociacao>? PropostasComoComprador { get; set; }
    }
}

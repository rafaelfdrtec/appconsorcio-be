using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.DTOs
{
    /// <summary>
    /// DTO para registrar um novo usuário
    /// </summary>
    public class RegistroUsuarioDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email em formato inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O papel do usuário é obrigatório")]
        [RegularExpression("^(admin|vendedor|comprador)$", ErrorMessage = "Papel inválido. Os valores permitidos são: admin, vendedor ou comprador")]
        public string Papel { get; set; } = "comprador";
    }

    /// <summary>
    /// DTO para login de usuário
    /// </summary>
    public class LoginUsuarioDTO
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email em formato inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para retornar informações do usuário autenticado
    /// </summary>
    public class UsuarioRespostaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;
        public bool EhVerificado { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}

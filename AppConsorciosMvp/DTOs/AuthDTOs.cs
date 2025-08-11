using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppConsorciosMvp.DTOs
{
    /// <summary>
    /// DTO para login via /auth/login com campos email e password
    /// </summary>
    public class AuthLoginRequest
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email em formato inválido")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}

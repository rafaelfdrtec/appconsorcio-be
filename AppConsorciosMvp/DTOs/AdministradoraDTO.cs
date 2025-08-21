using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.DTOs
{
    /// <summary>
    /// DTO para criação de administradora
    /// </summary>
    public class CriarAdministradoraDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$|^\d{14}$", 
            ErrorMessage = "CNPJ deve estar no formato válido")]
        public string Cnpj { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Email deve estar em formato válido")]
        [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [RegularExpression("^(ativa|inativa)$", ErrorMessage = "Status deve ser 'ativa' ou 'inativa'")]
        public string Status { get; set; } = "ativa";
    }

    /// <summary>
    /// DTO para atualização de administradora
    /// </summary>
    public class AtualizarAdministradoraDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$|^\d{14}$", 
            ErrorMessage = "CNPJ deve estar no formato válido")]
        public string Cnpj { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Email deve estar em formato válido")]
        [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [RegularExpression("^(ativa|inativa)$", ErrorMessage = "Status deve ser 'ativa' ou 'inativa'")]
        public string Status { get; set; } = "ativa";
    }

    /// <summary>
    /// DTO para resposta de administradora
    /// </summary>
    public class AdministradoraRespostaDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
    }
    
    public class AdministradoraSmallRespostaDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
}

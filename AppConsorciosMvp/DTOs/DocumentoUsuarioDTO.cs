using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.DTOs
{
    /// <summary>
    /// DTO para upload de documento
    /// </summary>
    public class UploadDocumentoDTO
    {
        [Required(ErrorMessage = "O tipo do documento é obrigatório")]
        [RegularExpression("^(RG|CPF|CNH|Passaporte|ComprovanteRenda|ComprovanteResidencia|Outro)$",
            ErrorMessage = "Tipo de documento inválido")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O arquivo é obrigatório")]
        public IFormFile Arquivo { get; set; } = null!;
    }

    /// <summary>
    /// DTO para resposta de documento
    /// </summary>
    public class DocumentoUsuarioRespostaDTO
    {
        public string Id { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string NomeArquivo { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ObservacoesValidacao { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
        public int? ValidadoPorId { get; set; }
        public string? NomeValidador { get; set; }
        public string? ValidadoEm { get; set; }
    }

    /// <summary>
    /// DTO para validação de documento
    /// </summary>
    public class ValidarDocumentoDTO
    {
        [Required(ErrorMessage = "O status é obrigatório")]
        [RegularExpression("^(aprovado|rejeitado)$", 
            ErrorMessage = "Status deve ser 'aprovado' ou 'rejeitado'")]
        public string Status { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
        public string? ObservacoesValidacao { get; set; }
    }

    /// <summary>
    /// DTO para download de documento
    /// </summary>
    public class DocumentoDownloadDTO
    {
        public byte[] Conteudo { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string NomeArquivo { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppConsorciosMvp.Models.Enums;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Representa um documento pessoal enviado pelo usuário
    /// </summary>
    public class DocumentoUsuario
    {
        /// <summary>
        /// Identificador único do documento
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID do usuário que enviou o documento
        /// </summary>
        [Required(ErrorMessage = "O usuário é obrigatório")]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Referência ao usuário
        /// </summary>
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        /// <summary>
        /// Tipo do documento (RG, CPF, CNH, etc.)
        /// </summary>
        [Required(ErrorMessage = "O tipo do documento é obrigatório")]
        public DocumentoTipo TipoDocumento { get; set; } = DocumentoTipo.Outro;

        /// <summary>
        /// Nome original do arquivo
        /// </summary>
        [Required(ErrorMessage = "O nome do arquivo é obrigatório")]
        [StringLength(255, ErrorMessage = "O nome do arquivo deve ter no máximo 255 caracteres")]
        public string NomeArquivo { get; set; } = string.Empty;

        /// <summary>
        /// URL do blob no Azure Storage
        /// </summary>
        [Required(ErrorMessage = "A URL do blob é obrigatória")]
        public string BlobUrl { get; set; } = string.Empty;

        /// <summary>
        /// Nome do blob no container
        /// </summary>
        [Required(ErrorMessage = "O nome do blob é obrigatório")]
        public string BlobName { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de conteúdo do arquivo (MIME type)
        /// </summary>
        [Required(ErrorMessage = "O tipo de conteúdo é obrigatório")]
        [StringLength(100, ErrorMessage = "O tipo de conteúdo deve ter no máximo 100 caracteres")]
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Tamanho do arquivo em bytes
        /// </summary>
        [Required(ErrorMessage = "O tamanho do arquivo é obrigatório")]
        [Range(1, long.MaxValue, ErrorMessage = "O tamanho do arquivo deve ser maior que zero")]
        public long TamanhoBytes { get; set; }

        /// <summary>
        /// Status da validação do documento
        /// </summary>
        [Required(ErrorMessage = "O status é obrigatório")]
        public DocumentoStatus Status { get; set; } = DocumentoStatus.Pendente; // pendente, aprovado, rejeitado

        /// <summary>
        /// Observações sobre a validação
        /// </summary>
        public string? ObservacoesValidacao { get; set; }

        /// <summary>
        /// Data de upload do documento
        /// </summary>
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID do administrador que validou (se aplicável)
        /// </summary>
        public int? ValidadoPorId { get; set; }

        /// <summary>
        /// Referência ao administrador que validou
        /// </summary>
        [ForeignKey("ValidadoPorId")]
        public Usuario? ValidadoPor { get; set; }

        /// <summary>
        /// Data da validação
        /// </summary>
        public DateTime? ValidadoEm { get; set; }
    }
}

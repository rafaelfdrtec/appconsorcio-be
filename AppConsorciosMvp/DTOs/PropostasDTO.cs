using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.DTOs
{
    public class CriarPropostaDTO
    {
        [Required]
        public int CartaConsorcioId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "O ágio deve ser maior ou igual a zero")]
        public decimal Agio { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O prazo em meses deve ser maior que zero")]
        public int? PrazoMeses { get; set; }
    }

    public class PropostaRespostaDTO
    {
        public int Id { get; set; }
        public int CartaConsorcioId { get; set; }
        public int CompradorId { get; set; }
        public string? NomeComprador { get; set; }
        public decimal Agio { get; set; }
        public int? PrazoMeses { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? MotivoCancelamento { get; set; }
        public string CriadaEm { get; set; } = string.Empty;
        public string? CanceladaEm { get; set; }
        public string? EfetivadaEm { get; set; }
    }

    public class CancelarPropostaDTO
    {
        [StringLength(500)]
        public string? Motivo { get; set; }
    }

    public class EfetivarPropostaDTO
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ValorVenda { get; set; }
    }

    public class UploadAnexoPropostaDTO
    {
        [Required]
        public IFormFile Arquivo { get; set; } = null!;
    }

    public class UploadAnexoCartaDTO
    {
        [Required]
        public IFormFile Arquivo { get; set; } = null!;
    }

    public class AnexoRespostaDTO
    {
        public string Id { get; set; } = string.Empty;
        public string NomeOriginal { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public string BlobName { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public string CriadoEm { get; set; } = string.Empty;
    }
}

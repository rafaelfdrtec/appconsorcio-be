using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.DTOs
{
    /// <summary>
    /// DTO para criar uma nova carta de consórcio
    /// </summary>
    public class CriarCartaConsorcioDTO
    {
        [Required(ErrorMessage = "O valor do crédito é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do crédito deve ser maior que zero")]
        public decimal ValorCredito { get; set; }

        [Required(ErrorMessage = "O valor da entrada é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da entrada deve ser maior que zero")]
        public decimal ValorEntrada { get; set; }

        [Required(ErrorMessage = "O número de parcelas pagas é obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "O número de parcelas pagas deve ser maior ou igual a zero")]
        public int ParcelasPagas { get; set; }

        [Required(ErrorMessage = "O número total de parcelas é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O número total de parcelas deve ser maior que zero")]
        public int ParcelasTotais { get; set; }

        [Required(ErrorMessage = "O valor da parcela é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da parcela deve ser maior que zero")]
        public decimal ValorParcela { get; set; }

        [Required(ErrorMessage = "O tipo de bem é obrigatório")]
        [RegularExpression("^(imovel|veiculo|outro)$", ErrorMessage = "Tipo de bem inválido. Os valores permitidos são: imovel, veiculo ou outro")]
        public string TipoBem { get; set; } = string.Empty;

        public string? Descricao { get; set; }
    }

    /// <summary>
    /// DTO para atualizar o status de uma carta de consórcio
    /// </summary>
    public class AtualizarStatusCartaDTO
    {
        [Required(ErrorMessage = "O status é obrigatório")]
        [RegularExpression("^(disponivel|negociando|vendida)$", ErrorMessage = "Status inválido. Os valores permitidos são: disponivel, negociando ou vendida")]
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para pesquisar cartas de consórcio
    /// </summary>
    public class PesquisarCartaConsorcioDTO
    {
        public string? TipoBem { get; set; }
        public decimal? ValorCreditoMinimo { get; set; }
        public decimal? ValorCreditoMaximo { get; set; }
        public int? ParcelasPagasMinimo { get; set; }
    }

    /// <summary>
    /// DTO para retornar informações de uma carta de consórcio
    /// </summary>
    public class CartaConsorcioRespostaDTO
    {
        public int Id { get; set; }
        public int VendedorId { get; set; }
        public string NomeVendedor { get; set; } = string.Empty;
        public decimal ValorCredito { get; set; }
        public decimal ValorEntrada { get; set; }
        public int ParcelasPagas { get; set; }
        public int ParcelasTotais { get; set; }
        public decimal ValorParcela { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TipoBem { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime CriadoEm { get; set; }
        public bool EhVerificado { get; set; }
    }
}

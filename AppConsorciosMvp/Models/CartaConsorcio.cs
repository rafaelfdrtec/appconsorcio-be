using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppConsorciosMvp.Models.Enums;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Representa uma carta de consórcio no sistema
    /// </summary>
    public class CartaConsorcio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O vendedor é obrigatório")]
        public int VendedorId { get; set; }

        [ForeignKey("VendedorId")]
        public Usuario? Vendedor { get; set; }

        [Required(ErrorMessage = "A administradora é obrigatória")]
        public int AdministradoraId { get; set; }

        [ForeignKey("AdministradoraId")]
        public Administradora? Administradora { get; set; }

        [Required(ErrorMessage = "O valor do crédito é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do crédito deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorCredito { get; set; }

        [Required(ErrorMessage = "O valor da entrada é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da entrada deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorEntrada { get; set; }

        [Required(ErrorMessage = "O número de parcelas pagas é obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "O número de parcelas pagas deve ser maior ou igual a zero")]
        public int ParcelasPagas { get; set; }

        [Required(ErrorMessage = "O número total de parcelas é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O número total de parcelas deve ser maior que zero")]
        public int ParcelasTotais { get; set; }

        [Required(ErrorMessage = "O valor da parcela é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da parcela deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorParcela { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        public CartaStatus Status { get; set; } = CartaStatus.Disponivel;

        [Required(ErrorMessage = "O tipo de bem é obrigatório")]
        public CartaTipoBem TipoBem { get; set; } = CartaTipoBem.Outro;

        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O número da cota é obrigatório")]
        [StringLength(20, ErrorMessage = "O número da cota deve ter no máximo 20 caracteres")]
        public string NumeroCota { get; set; } = string.Empty;

        [Required(ErrorMessage = "O grupo é obrigatório")]
        [StringLength(20, ErrorMessage = "O grupo deve ter no máximo 20 caracteres")]
        public string Grupo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de contemplação é obrigatório")]
        public CartaTipoContemplacao TipoContemplacao { get; set; } = CartaTipoContemplacao.Outro;

        public DateTime? DataContemplacao { get; set; }

        public string? Observacoes { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public bool EhVerificado { get; set; } = false;

        public int? CompradorId { get; set; }

        [ForeignKey("CompradorId")]
        public Usuario? Comprador { get; set; }

        public DateTime? DataVenda { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorVenda { get; set; }

        public int? PropostaVendaId { get; set; }

        [ForeignKey("PropostaVendaId")]
        public PropostaNegociacao? PropostaVenda { get; set; }

        public ICollection<PropostaNegociacao>? Propostas { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppConsorciosMvp.Models
{
    /// <summary>
    /// Representa uma carta de consórcio no sistema
    /// </summary>
    public class CartaConsorcio
    {
        /// <summary>
        /// Identificador único da carta
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID do usuário vendedor da carta
        /// </summary>
        [Required(ErrorMessage = "O vendedor é obrigatório")]
        public int VendedorId { get; set; }

        /// <summary>
        /// Referência ao usuário vendedor
        /// </summary>
        [ForeignKey("VendedorId")]
        public Usuario? Vendedor { get; set; }

        /// <summary>
        /// ID da administradora do consórcio
        /// </summary>
        [Required(ErrorMessage = "A administradora é obrigatória")]
        public Guid AdministradoraId { get; set; }

        /// <summary>
        /// Referência à administradora do consórcio
        /// </summary>
        [ForeignKey("AdministradoraId")]
        public Administradora? Administradora { get; set; }

        /// <summary>
        /// Valor total do crédito da carta
        /// </summary>
        [Required(ErrorMessage = "O valor do crédito é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do crédito deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorCredito { get; set; }

        /// <summary>
        /// Valor da entrada necessária
        /// </summary>
        [Required(ErrorMessage = "O valor da entrada é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da entrada deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorEntrada { get; set; }

        /// <summary>
        /// Número de parcelas já pagas
        /// </summary>
        [Required(ErrorMessage = "O número de parcelas pagas é obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "O número de parcelas pagas deve ser maior ou igual a zero")]
        public int ParcelasPagas { get; set; }

        /// <summary>
        /// Número total de parcelas do consórcio
        /// </summary>
        [Required(ErrorMessage = "O número total de parcelas é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O número total de parcelas deve ser maior que zero")]
        public int ParcelasTotais { get; set; }

        /// <summary>
        /// Valor da parcela mensal
        /// </summary>
        [Required(ErrorMessage = "O valor da parcela é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da parcela deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorParcela { get; set; }

        /// <summary>
        /// Status atual da carta (disponível, negociando, vendida)
        /// </summary>
        [Required(ErrorMessage = "O status é obrigatório")]
        public string Status { get; set; } = "disponivel";  // Valor padrão

        /// <summary>
        /// Tipo de bem que pode ser adquirido (imóvel, veículo, etc)
        /// </summary>
        [Required(ErrorMessage = "O tipo de bem é obrigatório")]
        public string TipoBem { get; set; } = string.Empty;

        /// <summary>
        /// Descrição adicional da carta
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Número da cota no consórcio
        /// </summary>
        [Required(ErrorMessage = "O número da cota é obrigatório")]
        [StringLength(20, ErrorMessage = "O número da cota deve ter no máximo 20 caracteres")]
        public string NumeroCota { get; set; } = string.Empty;

        /// <summary>
        /// Número do grupo do consórcio
        /// </summary>
        [Required(ErrorMessage = "O grupo é obrigatório")]
        [StringLength(20, ErrorMessage = "O grupo deve ter no máximo 20 caracteres")]
        public string Grupo { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de contemplação (sorteio ou lance)
        /// </summary>
        [Required(ErrorMessage = "O tipo de contemplação é obrigatório")]
        [StringLength(10, ErrorMessage = "O tipo de contemplação deve ter no máximo 10 caracteres")]
        public string TipoContemplacao { get; set; } = string.Empty;

        /// <summary>
        /// Data da contemplação (se aplicável)
        /// </summary>
        public DateTime? DataContemplacao { get; set; }

        /// <summary>
        /// Observações adicionais da carta
        /// </summary>
        public string? Observacoes { get; set; }

        /// <summary>
        /// Data de criação do registro da carta
        /// </summary>
        public DateTime CriadoEm { get; set; } = DateTime.Now;

        /// <summary>
        /// Indica se a carta foi verificada pela equipe administrativa
        /// </summary>
        public bool EhVerificado { get; set; } = false;  // Valor padrão
    }
}

using System.Security.Claims;
using AppConsorciosMvp.Data;
using AppConsorciosMvp.DTOs;
using AppConsorciosMvp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartasController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria uma nova carta de consórcio
        /// </summary>
        /// <param name="cartaDTO">Dados da carta</param>
        /// <returns>Carta criada</returns>
        [HttpPost]
        [Authorize(Roles = "vendedor,admin")] // Apenas vendedores ou admins podem criar cartas
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartaConsorcioRespostaDTO>> CriarCarta(CriarCartaConsorcioDTO cartaDTO)
        {
            // Obter ID do usuário autenticado
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Verificar se o usuário existe
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return BadRequest("Usuário não encontrado");
            }

            // Criar a carta
            var carta = new CartaConsorcio
            {
                VendedorId = usuarioId,
                ValorCredito = cartaDTO.ValorCredito,
                ValorEntrada = cartaDTO.ValorEntrada,
                ParcelasPagas = cartaDTO.ParcelasPagas,
                ParcelasTotais = cartaDTO.ParcelasTotais,
                ValorParcela = cartaDTO.ValorParcela,
                Status = "disponivel", // Status inicial
                TipoBem = cartaDTO.TipoBem,
                Descricao = cartaDTO.Descricao,
                CriadoEm = DateTime.Now,
                EhVerificado = false // Inicialmente não verificada
            };

            _context.CartasConsorcio.Add(carta);
            await _context.SaveChangesAsync();

            // Retornar a carta criada
            return CreatedAtAction(nameof(ObterCarta), new { id = carta.Id }, new CartaConsorcioRespostaDTO
            {
                Id = carta.Id,
                VendedorId = carta.VendedorId,
                NomeVendedor = usuario.Nome,
                ValorCredito = carta.ValorCredito,
                ValorEntrada = carta.ValorEntrada,
                ParcelasPagas = carta.ParcelasPagas,
                ParcelasTotais = carta.ParcelasTotais,
                ValorParcela = carta.ValorParcela,
                Status = carta.Status,
                TipoBem = carta.TipoBem,
                Descricao = carta.Descricao,
                CriadoEm = carta.CriadoEm,
                EhVerificado = carta.EhVerificado
            });
        }

        /// <summary>
        /// Obtém todas as cartas disponíveis
        /// </summary>
        /// <returns>Lista de cartas disponíveis</returns>
        [HttpGet]
        [AllowAnonymous] // Qualquer pessoa pode ver as cartas disponíveis
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CartaConsorcioRespostaDTO>>> ObterCartasDisponiveis()
        {
            // Buscar cartas com status "disponivel"
            var cartas = await _context.CartasConsorcio
                .Where(c => c.Status == "disponivel")
                .Include(c => c.Vendedor)
                .ToListAsync();

            // Mapear para DTO
            var cartasDTO = cartas.Select(c => new CartaConsorcioRespostaDTO
            {
                Id = c.Id,
                VendedorId = c.VendedorId,
                NomeVendedor = c.Vendedor?.Nome ?? "Desconhecido",
                ValorCredito = c.ValorCredito,
                ValorEntrada = c.ValorEntrada,
                ParcelasPagas = c.ParcelasPagas,
                ParcelasTotais = c.ParcelasTotais,
                ValorParcela = c.ValorParcela,
                Status = c.Status,
                TipoBem = c.TipoBem,
                Descricao = c.Descricao,
                CriadoEm = c.CriadoEm,
                EhVerificado = c.EhVerificado
            }).ToList();

            return Ok(cartasDTO);
        }

        /// <summary>
        /// Obtém os detalhes de uma carta específica
        /// </summary>
        /// <param name="id">ID da carta</param>
        /// <returns>Detalhes da carta</returns>
        [HttpGet("{id}")]
        [AllowAnonymous] // Qualquer pessoa pode ver os detalhes de uma carta
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartaConsorcioRespostaDTO>> ObterCarta(int id)
        {
            // Buscar carta pelo ID
            var carta = await _context.CartasConsorcio
                .Include(c => c.Vendedor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null)
            {
                return NotFound();
            }

            // Mapear para DTO
            var cartaDTO = new CartaConsorcioRespostaDTO
            {
                Id = carta.Id,
                VendedorId = carta.VendedorId,
                NomeVendedor = carta.Vendedor?.Nome ?? "Desconhecido",
                ValorCredito = carta.ValorCredito,
                ValorEntrada = carta.ValorEntrada,
                ParcelasPagas = carta.ParcelasPagas,
                ParcelasTotais = carta.ParcelasTotais,
                ValorParcela = carta.ValorParcela,
                Status = carta.Status,
                TipoBem = carta.TipoBem,
                Descricao = carta.Descricao,
                CriadoEm = carta.CriadoEm,
                EhVerificado = carta.EhVerificado
            };

            return Ok(cartaDTO);
        }

        /// <summary>
        /// Pesquisa cartas com filtros
        /// </summary>
        /// <param name="filtros">Filtros de pesquisa</param>
        /// <returns>Lista de cartas filtradas</returns>
        [HttpGet("pesquisar")]
        [AllowAnonymous] // Qualquer pessoa pode pesquisar cartas
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CartaConsorcioRespostaDTO>>> PesquisarCartas([FromQuery] PesquisarCartaConsorcioDTO filtros)
        {
            // Construir a consulta com base nos filtros
            IQueryable<CartaConsorcio> query = _context.CartasConsorcio
                .Where(c => c.Status == "disponivel") // Apenas cartas disponíveis
                .Include(c => c.Vendedor);

            // Aplicar filtros se fornecidos
            if (!string.IsNullOrEmpty(filtros.TipoBem))
            {
                query = query.Where(c => c.TipoBem == filtros.TipoBem);
            }

            if (filtros.ValorCreditoMinimo.HasValue)
            {
                query = query.Where(c => c.ValorCredito >= filtros.ValorCreditoMinimo.Value);
            }

            if (filtros.ValorCreditoMaximo.HasValue)
            {
                query = query.Where(c => c.ValorCredito <= filtros.ValorCreditoMaximo.Value);
            }

            if (filtros.ParcelasPagasMinimo.HasValue)
            {
                query = query.Where(c => c.ParcelasPagas >= filtros.ParcelasPagasMinimo.Value);
            }

            // Executar a consulta
            var cartas = await query.ToListAsync();

            // Mapear para DTO
            var cartasDTO = cartas.Select(c => new CartaConsorcioRespostaDTO
            {
                Id = c.Id,
                VendedorId = c.VendedorId,
                NomeVendedor = c.Vendedor?.Nome ?? "Desconhecido",
                ValorCredito = c.ValorCredito,
                ValorEntrada = c.ValorEntrada,
                ParcelasPagas = c.ParcelasPagas,
                ParcelasTotais = c.ParcelasTotais,
                ValorParcela = c.ValorParcela,
                Status = c.Status,
                TipoBem = c.TipoBem,
                Descricao = c.Descricao,
                CriadoEm = c.CriadoEm,
                EhVerificado = c.EhVerificado
            }).ToList();

            return Ok(cartasDTO);
        }

        /// <summary>
        /// Atualiza o status de uma carta
        /// </summary>
        /// <param name="id">ID da carta</param>
        /// <param name="statusDTO">Novo status</param>
        /// <returns>Carta atualizada</returns>
        [HttpPut("{id}/status")]
        [Authorize] // Requer autenticação
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CartaConsorcioRespostaDTO>> AtualizarStatusCarta(int id, AtualizarStatusCartaDTO statusDTO)
        {
            // Buscar carta pelo ID
            var carta = await _context.CartasConsorcio
                .Include(c => c.Vendedor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null)
            {
                return NotFound();
            }

            // Obter ID do usuário autenticado
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var papelUsuario = User.FindFirst(ClaimTypes.Role)?.Value;

            // Verificar se o usuário tem permissão para atualizar o status
            // Apenas o vendedor da carta ou um administrador pode fazer isso
            if (carta.VendedorId != usuarioId && papelUsuario != "admin")
            {
                return Forbid();
            }

            // Verificar se a transição de status é válida
            if (!EhTransicaoStatusValida(carta.Status, statusDTO.Status))
            {
                return BadRequest($"Transição de status inválida: {carta.Status} -> {statusDTO.Status}");
            }

            // Atualizar o status
            carta.Status = statusDTO.Status;
            await _context.SaveChangesAsync();

            // Mapear para DTO
            var cartaDTO = new CartaConsorcioRespostaDTO
            {
                Id = carta.Id,
                VendedorId = carta.VendedorId,
                NomeVendedor = carta.Vendedor?.Nome ?? "Desconhecido",
                ValorCredito = carta.ValorCredito,
                ValorEntrada = carta.ValorEntrada,
                ParcelasPagas = carta.ParcelasPagas,
                ParcelasTotais = carta.ParcelasTotais,
                ValorParcela = carta.ValorParcela,
                Status = carta.Status,
                TipoBem = carta.TipoBem,
                Descricao = carta.Descricao,
                CriadoEm = carta.CriadoEm,
                EhVerificado = carta.EhVerificado
            };

            return Ok(cartaDTO);
        }

        /// <summary>
        /// Verifica uma carta (marca como verificada)
        /// </summary>
        /// <param name="id">ID da carta</param>
        /// <returns>Carta verificada</returns>
        [HttpPost("{id}/verificar")]
        [Authorize(Roles = "admin")] // Apenas administradores podem verificar cartas
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartaConsorcioRespostaDTO>> VerificarCarta(int id)
        {
            // Buscar carta pelo ID
            var carta = await _context.CartasConsorcio
                .Include(c => c.Vendedor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null)
            {
                return NotFound();
            }

            // Marcar como verificada
            carta.EhVerificado = true;
            await _context.SaveChangesAsync();

            // Mapear para DTO
            var cartaDTO = new CartaConsorcioRespostaDTO
            {
                Id = carta.Id,
                VendedorId = carta.VendedorId,
                NomeVendedor = carta.Vendedor?.Nome ?? "Desconhecido",
                ValorCredito = carta.ValorCredito,
                ValorEntrada = carta.ValorEntrada,
                ParcelasPagas = carta.ParcelasPagas,
                ParcelasTotais = carta.ParcelasTotais,
                ValorParcela = carta.ValorParcela,
                Status = carta.Status,
                TipoBem = carta.TipoBem,
                Descricao = carta.Descricao,
                CriadoEm = carta.CriadoEm,
                EhVerificado = carta.EhVerificado
            };

            return Ok(cartaDTO);
        }

        /// <summary>
        /// Verifica se a transição de status é válida
        /// </summary>
        /// <param name="statusAtual">Status atual da carta</param>
        /// <param name="novoStatus">Novo status</param>
        /// <returns>True se a transição for válida, False caso contrário</returns>
        private bool EhTransicaoStatusValida(string statusAtual, string novoStatus)
        {
            // Regras de transição:
            // disponivel -> negociando -> vendida
            // também permite disponivel -> vendida (em casos especiais)
            // e negociando -> disponivel (caso a negociação falhe)

            switch (statusAtual)
            {
                case "disponivel":
                    return novoStatus == "negociando" || novoStatus == "vendida";

                case "negociando":
                    return novoStatus == "disponivel" || novoStatus == "vendida";

                case "vendida":
                    // Uma vez vendida, não pode mudar o status
                    return false;

                default:
                    return false;
            }
        }
    }
}

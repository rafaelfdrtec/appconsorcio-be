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
    public class CartasController(AppDbContext context) : ControllerBase
    {
        /// <summary>
        /// Cria uma carta de consórcio
        /// </summary>
        /// <param name="cartaDto">Dados da carta</param>
        /// <returns>Carta criada</returns>
        [HttpPost]
        [Authorize(Roles = "vendedor,admin")] // Apenas vendedores ou admins podem criar cartas
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartaConsorcioRespostaDTO>> CriarCarta(CriarCartaConsorcioDTO cartaDto)
        {
            // Obter ID do usuário autenticado
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Verificar se o usuário existe
            var usuario = await context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return BadRequest("Usuário não encontrado");
            }

            // Verificar se a administradora existe
            var administradora = await context.Administradoras.FindAsync(cartaDto.AdministradoraId);
            if (administradora == null)
            {
                return BadRequest("Administradora não encontrada");
            }

            // Verificar se a administradora está ativa
            if (administradora.Status != "ativa")
            {
                return BadRequest("A administradora selecionada não está ativa");
            }

            // Criar a carta
            var carta = new CartaConsorcio
            {
                VendedorId = usuarioId,
                AdministradoraId = cartaDto.AdministradoraId,
                ValorCredito = cartaDto.ValorCredito,
                ValorEntrada = cartaDto.ValorEntrada,
                ParcelasPagas = cartaDto.ParcelasPagas,
                ParcelasTotais = cartaDto.ParcelasTotais,
                ValorParcela = cartaDto.ValorParcela,
                Status = "disponivel", // Status inicial
                TipoBem = cartaDto.TipoBem,
                Descricao = cartaDto.Descricao,
                NumeroCota = cartaDto.NumeroCota,
                Grupo = cartaDto.Grupo,
                TipoContemplacao = cartaDto.TipoContemplacao,
                DataContemplacao = cartaDto.DataContemplacao,
                Observacoes = cartaDto.Observacoes,
                CriadoEm = DateTime.Now,
                EhVerificado = false // Inicialmente não verificada
            };

            context.CartasConsorcio.Add(carta);
            await context.SaveChangesAsync();

            // Retornar a carta criada
            return CreatedAtAction(nameof(ObterCarta), new { id = carta.Id }, new CartaConsorcioRespostaDTO
            {
                Id = carta.Id,
                VendedorId = carta.VendedorId,
                NomeVendedor = usuario.Nome,
                AdministradoraId = carta.AdministradoraId,
                NomeAdministradora = administradora.Nome,
                ValorCredito = carta.ValorCredito,
                ValorEntrada = carta.ValorEntrada,
                ParcelasPagas = carta.ParcelasPagas,
                ParcelasTotais = carta.ParcelasTotais,
                ValorParcela = carta.ValorParcela,
                Status = carta.Status,
                TipoBem = carta.TipoBem,
                Descricao = carta.Descricao,
                NumeroCota = carta.NumeroCota,
                Grupo = carta.Grupo,
                TipoContemplacao = carta.TipoContemplacao,
                DataContemplacao = carta.DataContemplacao,
                Observacoes = carta.Observacoes,
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
            var cartas = await context.CartasConsorcio
                .Where(c => c.Status == "disponivel")
                .Include(c => c.Vendedor)
                .Include(c => c.Administradora)
                .ToListAsync();

            // Mapear para DTO
            var cartasDto = cartas.Select(c => new CartaConsorcioRespostaDTO
            {
                Id = c.Id,
                VendedorId = c.VendedorId,
                NomeVendedor = c.Vendedor?.Nome ?? "Desconhecido",
                AdministradoraId = c.AdministradoraId,
                NomeAdministradora = c.Administradora?.Nome ?? "Desconhecida",
                ValorCredito = c.ValorCredito,
                ValorEntrada = c.ValorEntrada,
                ParcelasPagas = c.ParcelasPagas,
                ParcelasTotais = c.ParcelasTotais,
                ValorParcela = c.ValorParcela,
                Status = c.Status,
                TipoBem = c.TipoBem,
                Descricao = c.Descricao,
                NumeroCota = c.NumeroCota,
                Grupo = c.Grupo,
                TipoContemplacao = c.TipoContemplacao,
                DataContemplacao = c.DataContemplacao,
                Observacoes = c.Observacoes,
                CriadoEm = c.CriadoEm,
                EhVerificado = c.EhVerificado
            }).ToList();

            return Ok(cartasDto);
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
            var carta = await context.CartasConsorcio
                .Include(c => c.Vendedor)
                .Include(c => c.Administradora)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null)
            {
                return NotFound();
            }

            // Mapear para DTO
            var cartaDto = new CartaConsorcioRespostaDTO
            {
                Id = carta.Id,
                VendedorId = carta.VendedorId,
                NomeVendedor = carta.Vendedor?.Nome ?? "Desconhecido",
                AdministradoraId = carta.AdministradoraId,
                NomeAdministradora = carta.Administradora?.Nome ?? "Desconhecida",
                ValorCredito = carta.ValorCredito,
                ValorEntrada = carta.ValorEntrada,
                ParcelasPagas = carta.ParcelasPagas,
                ParcelasTotais = carta.ParcelasTotais,
                ValorParcela = carta.ValorParcela,
                Status = carta.Status,
                TipoBem = carta.TipoBem,
                Descricao = carta.Descricao,
                NumeroCota = carta.NumeroCota,
                Grupo = carta.Grupo,
                TipoContemplacao = carta.TipoContemplacao,
                DataContemplacao = carta.DataContemplacao,
                Observacoes = carta.Observacoes,
                CriadoEm = carta.CriadoEm,
                EhVerificado = carta.EhVerificado
            };

            return Ok(cartaDto);
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
            IQueryable<CartaConsorcio> query = context.CartasConsorcio
                .Where(c => c.Status == "disponivel") // Apenas cartas disponíveis
                .Include(c => c.Vendedor)
                .Include(c => c.Administradora);

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
            var cartasDto = cartas.Select(c => new CartaConsorcioRespostaDTO
            {
                Id = c.Id,
                VendedorId = c.VendedorId,
                NomeVendedor = c.Vendedor?.Nome ?? "Desconhecido",
                AdministradoraId = c.AdministradoraId,
                NomeAdministradora = c.Administradora?.Nome ?? "Desconhecida",
                ValorCredito = c.ValorCredito,
                ValorEntrada = c.ValorEntrada,
                ParcelasPagas = c.ParcelasPagas,
                ParcelasTotais = c.ParcelasTotais,
                ValorParcela = c.ValorParcela,
                Status = c.Status,
                TipoBem = c.TipoBem,
                Descricao = c.Descricao,
                NumeroCota = c.NumeroCota,
                Grupo = c.Grupo,
                TipoContemplacao = c.TipoContemplacao,
                DataContemplacao = c.DataContemplacao,
                Observacoes = c.Observacoes,
                CriadoEm = c.CriadoEm,
                EhVerificado = c.EhVerificado
            }).ToList();

            return Ok(cartasDto);
        }

        /// <summary>
        /// Atualiza o status de uma carta
        /// </summary>
        /// <param name="id">ID da carta</param>
        /// <param name="statusDto">Novo status</param>
        /// <returns>Carta atualizada</returns>
        [HttpPut("{id}/status")]
        [Authorize] // Requer autenticação
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CartaConsorcioRespostaDTO>> AtualizarStatusCarta(int id, AtualizarStatusCartaDTO statusDto)
        {
            // Buscar carta pelo ID
            var carta = await context.CartasConsorcio
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
            if (!EhTransicaoStatusValida(carta.Status, statusDto.Status))
            {
                return BadRequest($"Transição de status inválida: {carta.Status} -> {statusDto.Status}");
            }

            // Atualizar o status
            carta.Status = statusDto.Status;
            await context.SaveChangesAsync();

            // Mapear para DTO
            var cartaDto = new CartaConsorcioRespostaDTO
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

            return Ok(cartaDto);
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
            var carta = await context.CartasConsorcio
                .Include(c => c.Vendedor)
                .Include(c => c.Administradora)
                .Include(c => c.Administradora)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null)
            {
                return NotFound();
            }

            // Marcar como verificada
            carta.EhVerificado = true;
            await context.SaveChangesAsync();

            // Mapear para DTO
            var cartaDto = new CartaConsorcioRespostaDTO
            {
                Id = carta.Id,
                VendedorId = carta.VendedorId,
                NomeVendedor = carta.Vendedor?.Nome ?? "Desconhecido",
                AdministradoraId = carta.AdministradoraId,
                NomeAdministradora = carta.Administradora?.Nome ?? "Desconhecida",
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

            return Ok(cartaDto);
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

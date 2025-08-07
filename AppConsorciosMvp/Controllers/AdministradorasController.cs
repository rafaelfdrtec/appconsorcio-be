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
    [Authorize] // Requer autenticação
    public class AdministradorasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdministradorasController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as administradoras (apenas para administradores)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdministradoraRespostaDTO>>> ListarAdministradoras()
        {
            if (!IsUserAdmin())
            {
                return Forbid("Acesso negado. Apenas administradores podem acessar este recurso.");
            }

            var administradoras = await _context.Administradoras
                .OrderBy(a => a.Nome)
                .ToListAsync();

            var result = administradoras.Select(a => new AdministradoraRespostaDTO
            {
                Id = a.Id.ToString(),
                Nome = a.Nome,
                Cnpj = a.Cnpj,
                Telefone = a.Telefone,
                Email = a.Email,
                Status = a.Status,
                CreatedAt = a.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt = a.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return Ok(result);
        }

        /// <summary>
        /// Busca uma administradora específica por ID (apenas para administradores)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdministradoraRespostaDTO>> BuscarAdministradora(Guid id)
        {
            if (!IsUserAdmin())
            {
                return Forbid("Acesso negado. Apenas administradores podem acessar este recurso.");
            }

            var administradora = await _context.Administradoras.FindAsync(id);

            if (administradora == null)
            {
                return NotFound("Administradora não encontrada.");
            }

            var result = new AdministradoraRespostaDTO
            {
                Id = administradora.Id.ToString(),
                Nome = administradora.Nome,
                Cnpj = administradora.Cnpj,
                Telefone = administradora.Telefone,
                Email = administradora.Email,
                Status = administradora.Status,
                CreatedAt = administradora.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt = administradora.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Ok(result);
        }

        /// <summary>
        /// Cria uma nova administradora (apenas para administradores)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AdministradoraRespostaDTO>> CriarAdministradora(CriarAdministradoraDTO dto)
        {
            if (!IsUserAdmin())
            {
                return Forbid("Acesso negado. Apenas administradores podem acessar este recurso.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se já existe uma administradora com o mesmo CNPJ
            var cnpjExiste = await _context.Administradoras
                .AnyAsync(a => a.Cnpj == dto.Cnpj);

            if (cnpjExiste)
            {
                return Conflict("Já existe uma administradora cadastrada com este CNPJ.");
            }

            var administradora = new Administradora
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Administradoras.Add(administradora);
            await _context.SaveChangesAsync();

            var result = new AdministradoraRespostaDTO
            {
                Id = administradora.Id.ToString(),
                Nome = administradora.Nome,
                Cnpj = administradora.Cnpj,
                Telefone = administradora.Telefone,
                Email = administradora.Email,
                Status = administradora.Status,
                CreatedAt = administradora.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt = administradora.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreatedAtAction(nameof(BuscarAdministradora), new { id = administradora.Id }, result);
        }

        /// <summary>
        /// Atualiza uma administradora existente (apenas para administradores)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAdministradora(Guid id, AtualizarAdministradoraDTO dto)
        {
            if (!IsUserAdmin())
            {
                return Forbid("Acesso negado. Apenas administradores podem acessar este recurso.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var administradora = await _context.Administradoras.FindAsync(id);

            if (administradora == null)
            {
                return NotFound("Administradora não encontrada.");
            }

            // Verificar se o CNPJ já existe em outra administradora
            var cnpjExiste = await _context.Administradoras
                .AnyAsync(a => a.Cnpj == dto.Cnpj && a.Id != id);

            if (cnpjExiste)
            {
                return Conflict("Já existe outra administradora cadastrada com este CNPJ.");
            }

            administradora.Nome = dto.Nome;
            administradora.Cnpj = dto.Cnpj;
            administradora.Telefone = dto.Telefone;
            administradora.Email = dto.Email;
            administradora.Status = dto.Status;
            administradora.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministradoraExists(id))
                {
                    return NotFound("Administradora não encontrada.");
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Exclui uma administradora (apenas para administradores)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirAdministradora(Guid id)
        {
            if (!IsUserAdmin())
            {
                return Forbid("Acesso negado. Apenas administradores podem acessar este recurso.");
            }

            var administradora = await _context.Administradoras.FindAsync(id);

            if (administradora == null)
            {
                return NotFound("Administradora não encontrada.");
            }

            // Verificar se existem cartas de consórcio vinculadas a esta administradora
            var possuiCartas = await _context.CartasConsorcio
                .AnyAsync(c => c.AdministradoraId == id);

            if (possuiCartas)
            {
                return BadRequest("Não é possível excluir esta administradora pois existem cartas de consórcio vinculadas a ela.");
            }

            _context.Administradoras.Remove(administradora);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdministradoraExists(Guid id)
        {
            return _context.Administradoras.Any(e => e.Id == id);
        }

        private bool IsUserAdmin()
        {
            var papel = User.FindFirst(ClaimTypes.Role)?.Value;
            return papel == "admin";
        }
    }
}

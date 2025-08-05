using System.Security.Claims;
using AppConsorciosMvp.Data;
using AppConsorciosMvp.DTOs;
using AppConsorciosMvp.Models;
using AppConsorciosMvp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHashService _passwordHashService;
        private readonly TokenService _tokenService;

        public UsuariosController(AppDbContext context, PasswordHashService passwordHashService, TokenService tokenService)
        {
            _context = context;
            _passwordHashService = passwordHashService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Endpoint para registrar um novo usuário
        /// </summary>
        /// <param name="registroDTO">Dados de registro do usuário</param>
        /// <returns>Dados do usuário criado e token JWT</returns>
        [HttpPost("registrar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioRespostaDTO>> Registrar(RegistroUsuarioDTO registroDTO)
        {
            // Verificar se o email já está em uso
            if (await _context.Usuarios.AnyAsync(u => u.Email == registroDTO.Email))
            {
                return BadRequest("Email já está em uso");
            }

            // Criar um novo usuário
            var usuario = new Usuario
            {
                Nome = registroDTO.Nome,
                Email = registroDTO.Email,
                SenhaHash = _passwordHashService.CriarHash(registroDTO.Senha),
                Papel = registroDTO.Papel,
                EhVerificado = false // Por padrão, o usuário não é verificado
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Gerar token JWT
            var token = _tokenService.GerarToken(usuario);

            // Retornar dados do usuário e token
            return CreatedAtAction(nameof(ObterUsuario), new { id = usuario.Id }, new UsuarioRespostaDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Papel = usuario.Papel,
                EhVerificado = usuario.EhVerificado,
                Token = token
            });
        }

        /// <summary>
        /// Endpoint para autenticar um usuário
        /// </summary>
        /// <param name="loginDTO">Credenciais de login</param>
        /// <returns>Dados do usuário e token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UsuarioRespostaDTO>> Login(LoginUsuarioDTO loginDTO)
        {
            // Buscar usuário pelo email
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (usuario == null)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            // Verificar senha
            if (!_passwordHashService.VerificarSenha(loginDTO.Senha, usuario.SenhaHash))
            {
                return Unauthorized("Email ou senha inválidos");
            }

            // Gerar token JWT
            var token = _tokenService.GerarToken(usuario);

            // Retornar dados do usuário e token
            return Ok(new UsuarioRespostaDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Papel = usuario.Papel,
                EhVerificado = usuario.EhVerificado,
                Token = token
            });
        }

        /// <summary>
        /// Endpoint para obter informações de um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        [HttpGet("{id}")]
        [Authorize] // Requer autenticação
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioRespostaDTO>> ObterUsuario(int id)
        {
            // Obter ID do usuário autenticado
            var usuarioAutenticadoId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var papelUsuario = User.FindFirst(ClaimTypes.Role)?.Value;

            // Verificar se o usuário tem permissão para acessar os dados
            // Apenas o próprio usuário ou um administrador pode ver os detalhes
            if (usuarioAutenticadoId != id && papelUsuario != "admin")
            {
                return Forbid();
            }

            // Buscar usuário
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Retornar dados do usuário (sem token)
            return Ok(new UsuarioRespostaDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Papel = usuario.Papel,
                EhVerificado = usuario.EhVerificado,
                Token = "" // Não incluímos token em operações de consulta
            });
        }

        /// <summary>
        /// Endpoint para verificar um usuário (marcar como verificado)
        /// </summary>
        /// <param name="id">ID do usuário a ser verificado</param>
        /// <returns>Dados atualizados do usuário</returns>
        [HttpPost("{id}/verificar")]
        [Authorize(Roles = "admin")] // Apenas administradores podem verificar usuários
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioRespostaDTO>> VerificarUsuario(int id)
        {
            // Buscar usuário
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Marcar como verificado
            usuario.EhVerificado = true;
            await _context.SaveChangesAsync();

            // Retornar dados do usuário
            return Ok(new UsuarioRespostaDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Papel = usuario.Papel,
                EhVerificado = usuario.EhVerificado,
                Token = "" // Não incluímos token em operações de consulta
            });
        }
    }
}

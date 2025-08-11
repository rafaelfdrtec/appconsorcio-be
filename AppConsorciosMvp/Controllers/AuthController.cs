using AppConsorciosMvp.Data;
using AppConsorciosMvp.DTOs;
using AppConsorciosMvp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHashService _passwordHashService;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, PasswordHashService passwordHashService, TokenService tokenService)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Autentica um usuário com email e password e retorna um token JWT.
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Dados do usuário autenticado e token JWT</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioRespostaDTO>> Login([FromBody] AuthLoginRequest request)
    {
        // Buscar usuário pelo email
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (usuario == null)
        {
            return Unauthorized("Email ou senha inválidos");
        }

        // Verificar senha
        if (!_passwordHashService.VerificarSenha(request.Password, usuario.SenhaHash))
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
}
using AppConsorciosMvp.Data;
using AppConsorciosMvp.DTOs;
using AppConsorciosMvp.Models;
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
    private readonly GoogleTokenValidationService _googleTokenValidationService;

    public AuthController(
        AppDbContext context,
        PasswordHashService passwordHashService,
        TokenService tokenService,
        GoogleTokenValidationService googleTokenValidationService)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _tokenService = tokenService;
        _googleTokenValidationService = googleTokenValidationService;
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

    /// <summary>
    /// Autentica via Google (NextAuth) usando o id_token e retorna nosso JWT.
    /// Se o usuário não existir, faz o provisionamento automático.
    /// </summary>
    /// <param name="request">Objeto contendo o id_token do Google</param>
    /// <returns>Dados do usuário autenticado e token JWT</returns>
    [HttpPost("google")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioRespostaDTO>> LoginComGoogle([FromBody] GoogleAuthRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.IdToken))
        {
            return BadRequest("IdToken é obrigatório.");
        }

        var tokenInfo = await _googleTokenValidationService.ValidateAsync(request.IdToken);
        if (tokenInfo == null)
        {
            return Unauthorized("Token do Google inválido.");
        }

        // Tenta localizar usuário pelo email
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == tokenInfo.Email);

        // Provisiona automaticamente se não existir
        if (usuario == null)
        {
            usuario = new Usuario
            {
                Nome = string.IsNullOrWhiteSpace(tokenInfo.Name)
                    ? tokenInfo.Email.Split('@')[0]
                    : tokenInfo.Name,
                Email = tokenInfo.Email,
                Papel = "Usuario",
                EhVerificado = tokenInfo.EmailVerified
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        // Emite nosso JWT
        var token = _tokenService.GerarToken(usuario);

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
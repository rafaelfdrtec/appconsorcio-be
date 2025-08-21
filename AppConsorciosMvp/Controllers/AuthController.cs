using AppConsorciosMvp.Data;
using AppConsorciosMvp.DTOs;
using AppConsorciosMvp.Models;
using AppConsorciosMvp.Models.Enums;
using AppConsorciosMvp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public class AuthController(
    AppDbContext context,
    PasswordHashService passwordHashService,
    TokenService tokenService,
    IConfiguration configuration)
    : ControllerBase
{
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
        var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (usuario == null)
        {
            return Unauthorized("Email ou senha inválidos");
        }

        // Verificar senha
        if (!passwordHashService.VerificarSenha(request.Password, usuario.SenhaHash))
        {
            return Unauthorized("Email ou senha inválidos");
        }

        // Gerar token JWT
        var token = tokenService.GerarToken(usuario);

        // Retornar dados do usuário e token
        return Ok(new UsuarioRespostaDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Papel = PapelToString(usuario.Papel),
            EhVerificado = usuario.EhVerificado,
            Token = token
        });
    }

    /// <summary>
    /// Autentica via Google (NextAuth) usando o id_token e retorna nosso JWT.
    /// Se o usuário não existir, faz o provisionamento automático.
    /// </summary>
    /// <param name="request">Objeto contendo o id_token do Google e dados auxiliares do perfil</param>
    /// <returns>Objeto com 'user' e 'tokens' (accessToken e refreshToken)</returns>
    [HttpPost("google")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDTO>> LoginComGoogle([FromBody] GoogleAuthRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.IdToken))
        {
            return BadRequest("IdToken é obrigatório.");
        }

        // Instancia o validador usando IConfiguration (sem depender de registro no DI)
        var tokenInfo = await new GoogleTokenValidationService(configuration).ValidateAsync(request.IdToken);
        if (tokenInfo == null)
        {
            return Unauthorized("Token do Google inválido.");
        }

        // Coerência: se o frontend enviou googleId/email, devem bater com o id_token
        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !string.Equals(request.Email, tokenInfo.Email, StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized("Email do payload não confere com o id_token.");
        }
        if (!string.IsNullOrWhiteSpace(request.GoogleId) &&
            !string.Equals(request.GoogleId, tokenInfo.Sub, StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized("GoogleId do payload não confere com o id_token.");
        }

        // Tenta localizar usuário pelo email do id_token (fonte de verdade)
        var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == tokenInfo.Email);

        // Provisiona automaticamente se não existir
        if (usuario == null)
        {
            // Preferir o nome enviado pelo frontend; se ausente, usar o do token; fallback: parte do email
            var nome = !string.IsNullOrWhiteSpace(request.Name)
                ? request.Name!
                : (!string.IsNullOrWhiteSpace(tokenInfo.Name)
                    ? tokenInfo.Name
                    : tokenInfo.Email.Split('@')[0]);

            usuario = new Usuario
            {
                Nome = nome,
                Email = tokenInfo.Email,
                Papel = UsuarioPapel.Comprador,
                EhVerificado = tokenInfo.EmailVerified
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
        }

        // Emite nosso JWT (access token)
        var accessToken = tokenService.GerarToken(usuario);

        // Se você ainda não tem refresh token implementado,
        // pode retornar null e o frontend tratará como opcional.
        string? refreshToken = null;

        // Monta resposta no formato esperado pelo frontend
        var response = new AuthResponseDTO
        {
            User = new BackendUserDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Papel = PapelToString(usuario.Papel),
                EhVerificado = usuario.EhVerificado,
                Avatar = request.Avatar // se quiser, pode persistir esse campo no futuro
            },
            Tokens = new AuthTokensDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };

        return Ok(response);
    }

    private static string PapelToString(UsuarioPapel papel) =>
        papel switch
        {
            UsuarioPapel.Administrador => "admin",
            UsuarioPapel.Vendedor => "vendedor",
            _ => "comprador"
        };
}
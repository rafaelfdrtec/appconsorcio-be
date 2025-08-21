using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppConsorciosMvp.Models;
using AppConsorciosMvp.Models.Enums;
using Microsoft.IdentityModel.Tokens;

namespace AppConsorciosMvp.Services
{
    /// <summary>
    /// Serviço para geração de tokens JWT
    /// </summary>
    public class TokenService(IConfiguration configuration)
    {
        /// <summary>
        /// Gera um token JWT para o usuário
        /// </summary>
        /// <param name="usuario">Usuário para o qual o token será gerado</param>
        /// <returns>Token JWT</returns>
        public string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado"));

            // Normaliza o papel para minúsculas nos claims (compatível com [Authorize(Roles = "...")])
            var role = usuario.Papel switch
            {
                UsuarioPapel.Administrador => "admin",
                UsuarioPapel.Vendedor => "vendedor",
                UsuarioPapel.Comprador => "comprador",
                _ => "comprador"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim("EhVerificado", usuario.EhVerificado.ToString()),
                new Claim("KycLevel", usuario.KycLevel.ToString()),
                new Claim("MfaEnabled", usuario.MfaEnabled.ToString())
            };

            var issuer = configuration["JWT:Issuer"] ?? "webapp";
            var audience = configuration["JWT:Audience"] ?? "consortium-api";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppConsorciosMvp.Models;
using Microsoft.IdentityModel.Tokens;

namespace AppConsorciosMvp.Services
{
    /// <summary>
    /// Serviço para geração de tokens JWT
    /// </summary>
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gera um token JWT para o usuário
        /// </summary>
        /// <param name="usuario">Usuário para o qual o token será gerado</param>
        /// <returns>Token JWT</returns>
        public string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado"));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Papel),
                new Claim("EhVerificado", usuario.EhVerificado.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

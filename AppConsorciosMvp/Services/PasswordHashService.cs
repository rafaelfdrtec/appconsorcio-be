using System.Security.Cryptography;
using System.Text;

namespace AppConsorciosMvp.Services
{
    /// <summary>
    /// Serviço para hash e verificação de senhas
    /// </summary>
    public class PasswordHashService
    {
        /// <summary>
        /// Cria um hash para a senha
        /// </summary>
        /// <param name="senha">Senha em texto puro</param>
        /// <returns>Hash da senha</returns>
        public string CriarHash(string senha)
        {
            // Usando HMACSHA512 para criar um hash mais seguro
            using var hmac = new HMACSHA512();

            // Salvamos a chave (salt) concatenada com o hash
            var senhaBytes = Encoding.UTF8.GetBytes(senha);
            var hashBytes = hmac.ComputeHash(senhaBytes);

            // Convertemos o salt e o hash em uma única string Base64
            var saltBase64 = Convert.ToBase64String(hmac.Key);
            var hashBase64 = Convert.ToBase64String(hashBytes);

            return $"{saltBase64}:{hashBase64}";
        }

        /// <summary>
        /// Verifica se a senha fornecida corresponde ao hash armazenado
        /// </summary>
        /// <param name="senha">Senha em texto puro</param>
        /// <param name="hashArmazenado">Hash armazenado</param>
        /// <returns>True se a senha for válida, False caso contrário</returns>
        public bool VerificarSenha(string senha, string hashArmazenado)
        {
            var partes = hashArmazenado.Split(':');
            if (partes.Length != 2)
                return false;

            var saltBase64 = partes[0];
            var hashBase64 = partes[1];

            try
            {
                var salt = Convert.FromBase64String(saltBase64);
                var hashArmazenadoBytes = Convert.FromBase64String(hashBase64);

                // Recriamos o hash com a mesma chave (salt)
                using var hmac = new HMACSHA512(salt);
                var senhaBytes = Encoding.UTF8.GetBytes(senha);
                var hashComputado = hmac.ComputeHash(senhaBytes);

                // Comparamos os hashes
                return hashComputado.SequenceEqual(hashArmazenadoBytes);
            }
            catch
            {
                return false;
            }
        }
    }
}

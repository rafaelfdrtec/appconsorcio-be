using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Services
{
    /// <summary>
    /// Serviço para leitura de parâmetros do sistema a partir do banco, com cache em memória
    /// </summary>
    public class ParametrosService
    {
        private readonly AppDbContext _db;
        private readonly Dictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);

        public ParametrosService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<string?> GetValorAsync(string chave)
        {
            if (_cache.TryGetValue(chave, out var valor))
                return valor;

            var param = await _db.ParametrosSistema.AsNoTracking().FirstOrDefaultAsync(p => p.Chave == chave);
            if (param != null)
            {
                _cache[chave] = param.Valor;
                return param.Valor;
            }
            return null;
        }

        public async Task<string> GetValorOrDefaultAsync(string chave, string valorDefault)
        {
            var valor = await GetValorAsync(chave);
            return string.IsNullOrWhiteSpace(valor) ? valorDefault : valor!;
        }

        public void Invalidate(string chave)
        {
            _cache.Remove(chave);
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}

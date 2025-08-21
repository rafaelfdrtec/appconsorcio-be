using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AppConsorciosMvp.Services
{
    /// <summary>
    /// Valida o id_token do Google consultando o endpoint oficial (tokeninfo) e
    /// checa o aud contra os ClientIds configurados em:
    /// - Google:ClientId (string única) ou
    /// - Google:ClientIds (array)
    /// </summary>
    public class GoogleTokenValidationService(IConfiguration config)
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public class GoogleTokenInfo
        {
            public string Sub { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public bool EmailVerified { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Picture { get; set; } = string.Empty;
            public string Audience { get; set; } = string.Empty;
        }

        public async Task<GoogleTokenInfo?> ValidateAsync(string idToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return null;

            var url = "https://oauth2.googleapis.com/tokeninfo?id_token=" + WebUtility.UrlEncode(idToken);
            using var resp = await _httpClient.GetAsync(url, cancellationToken);
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string aud = root.TryGetProperty("aud", out var audEl) ? (audEl.GetString() ?? string.Empty) : string.Empty;
            var allowed = GetAllowedAudiences();
            if (allowed.Length > 0 && !allowed.Contains(aud))
            {
                // ClientId não confere
                return null;
            }

            string email = root.TryGetProperty("email", out var eEl) ? (eEl.GetString() ?? string.Empty) : string.Empty;

            bool emailVerified = false;
            if (root.TryGetProperty("email_verified", out var evEl))
            {
                if (evEl.ValueKind == JsonValueKind.String)
                {
                    var s = evEl.GetString();
                    emailVerified = string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1";
                }
                else if (evEl.ValueKind == JsonValueKind.True || evEl.ValueKind == JsonValueKind.False)
                {
                    emailVerified = evEl.GetBoolean();
                }
            }

            string name = root.TryGetProperty("name", out var nEl) ? (nEl.GetString() ?? string.Empty) : string.Empty;
            string picture = root.TryGetProperty("picture", out var pEl) ? (pEl.GetString() ?? string.Empty) : string.Empty;
            string sub = root.TryGetProperty("sub", out var sEl) ? (sEl.GetString() ?? string.Empty) : string.Empty;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sub))
                return null;

            return new GoogleTokenInfo
            {
                Sub = sub,
                Email = email,
                EmailVerified = emailVerified,
                Name = name,
                Picture = picture,
                Audience = aud
            };
        }

        private string[] GetAllowedAudiences()
        {
            var list = config.GetSection("Google:ClientIds").Get<string[]>();
            if (list is { Length: > 0 })
                return list;

            var single = config["Google:ClientId"];
            if (!string.IsNullOrWhiteSpace(single))
                return new[] { single };

            return Array.Empty<string>();
        }
    }
}

using System.Text.Json.Serialization;

namespace AppConsorciosMvp.DTOs
{
    public class GoogleAuthRequest
    {
        // id_token retornado pelo Google (obrigatório para validação server-side)
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; } = string.Empty;

        // Campos informados pelo frontend (usados para coerência e preenchimento de perfil)
        [JsonPropertyName("googleId")]
        public string? GoogleId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }
    }
}

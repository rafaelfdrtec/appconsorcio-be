namespace AppConsorciosMvp.DTOs
{
    public class AuthResponseDTO
    {
        public BackendUserDTO User { get; set; } = default!;
        public AuthTokensDTO Tokens { get; set; } = default!;
    }

    public class BackendUserDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;
        public bool EhVerificado { get; set; }
        public string? Avatar { get; set; }
    }

    public class AuthTokensDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
    }
}

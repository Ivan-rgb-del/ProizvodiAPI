namespace ProizvodiAPI.Application.DTOs
{
    public class AuthDTO
    {
        public class LoginDTO
        {
            public string KorisnickoIme { get; set; } = string.Empty;

            public string Lozinka { get; set; } = string.Empty;
        }

        public class TokenResponseDTO
        {
            public string Token { get; set; } = string.Empty;

            public DateTime Expiry { get; set; }
        }
    }
}
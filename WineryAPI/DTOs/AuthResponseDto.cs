namespace WineryAPI.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Ime { get; set; } = string.Empty;
        public string Prez { get; set; } = string.Empty;
        public string Kategorija { get; set; } = string.Empty;
        public int Idzap { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}


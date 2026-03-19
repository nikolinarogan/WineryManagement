using WineryAPI.Models;

namespace WineryAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(Zaposleni zaposleni);
    }
}


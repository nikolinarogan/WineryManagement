using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface IAuthRepository
    {
        Task<Zaposleni?> GetZaposleniByEmailAsync(string email);
        Task<Zaposleni?> GetZaposleniByIdAsync(int id);
        Task<Zaposleni?> GetZaposleniWithDetailsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> JmbgExistsAsync(string jmbg);
        Task<Zaposleni> AddZaposleniAsync(Zaposleni zaposleni);
        Task AddEnologAsync(Enolog enolog);
        Task AddRadnikAsync(Radnik radnik);
        Task AddSomleijerAsync(Somleijer somleijer);
        Task UpdateZaposleniAsync(Zaposleni zaposleni);
        Task<List<Zaposleni>> GetAllZaposleniAsync(string? kategorija = null);
        Task SaveChangesAsync();
    }
}


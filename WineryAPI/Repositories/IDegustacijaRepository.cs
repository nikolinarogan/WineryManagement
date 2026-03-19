using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface IDegustacijaRepository
    {
        Task<List<Degustacija>> GetAllDegustacijeAsync();
        Task<Degustacija?> GetDegustacijaByIdAsync(int id);
        Task<Degustacija> CreateDegustacijaAsync(Degustacija degustacija);
        Task UpdateDegustacijaAsync(Degustacija degustacija);
        Task DeleteDegustacijaAsync(int id);
        Task<Somleijer?> GetSomleijerByIdAsync(int idzap);
        Task<bool> DegustacijaExistsAsync(int id);
    }
}


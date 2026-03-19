using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface ISirovovinoRepository
    {
        // Sirovovino CRUD
        Task<List<Sirovovino>> GetAllSirovovinaAsync();
        Task<Sirovovino?> GetSirovovinoByIdAsync(int id);
        Task<Sirovovino?> GetSirovovinoWithDetailsAsync(int id);
        Task<Sirovovino?> GetSirovovinoForDeletionAsync(int id);
        Task AddSirovovinoAsync(Sirovovino sirovovino);
        Task UpdateSirovovinoAsync(Sirovovino sirovovino);
        Task DeleteSirovovinoAsync(Sirovovino sirovovino);

        // Ubranasirovina queries
        Task<Ubranasirovina?> GetUbranaSirovinaForValidationAsync(int id);
        
        // Validacija
        Task<bool> SirovoVinoExistsForUbranaSirovinaAsync(int ubranasirovinaId);

        // JeOsnova queries (za praćenje iskorišćenosti grožđa)
        Task<decimal> GetIskoriscenoGrozdje(int ubranasirovinaId);

        // JeOsnova CRUD
        Task AddJeOsnovaAsync(JeOsnova jeOsnova);
    }
}


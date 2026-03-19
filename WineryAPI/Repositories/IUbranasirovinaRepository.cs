using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface IUbranasirovinaRepository
    {
        // Ubranasirovina CRUD
        Task<List<Ubranasirovina>> GetAllUbranasirovinaAsync();
        Task<Ubranasirovina?> GetUbranasirovinaByIdAsync(int id);
        Task<Ubranasirovina?> GetUbranasirovinaWithDetailsAsync(int id);
        Task AddUbranasirovinaAsync(Ubranasirovina ubranasirovina);
        Task UpdateUbranasirovinaAsync(Ubranasirovina ubranasirovina);
        Task DeleteUbranasirovinaAsync(Ubranasirovina ubranasirovina);

        // Rasporedbranja queries
        Task<List<Rasporedbranja>> GetRasporedbranjaForPrijemAsync();
        Task<Rasporedbranja?> GetRasporedbranjaWithFullDetailsAsync(int rasporedId);
        Task<Rasporedbranja?> GetRasporedbranjaForValidationAsync(int rasporedId);
    }
}


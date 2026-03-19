using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface IVinoRepository
    {
        // Vino CRUD
        Task<List<Vino>> GetAllVinaAsync();
        Task<Vino?> GetVinoByIdAsync(int id);
        Task<Vino?> GetVinoWithSirovimVinimaAsync(int id);
        Task<Vino?> GetVinoForDeletionAsync(int id);
        Task<bool> VinoExistsByNameAsync(string naziv, int? excludeId = null);
        Task AddVinoAsync(Vino vino);
        Task UpdateVinoAsync(Vino vino);
        Task DeleteVinoAsync(Vino vino);

        // Sirovovino queries
        Task<List<Sirovovino>> GetSirovaVinaByIdsAsync(List<int> ids);
    }
}


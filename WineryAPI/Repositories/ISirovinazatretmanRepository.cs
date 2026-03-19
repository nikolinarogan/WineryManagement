using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface ISirovinazatretmanRepository
    {
        // Sirovinazatretman CRUD
        Task<List<Sirovinazatretman>> GetAllSirovineAsync();
        Task<Sirovinazatretman?> GetSirovinaByIdAsync(int id);
        Task<Sirovinazatretman?> GetSirovinaWithUsageAsync(int id);
        Task<bool> SirovinaExistsByNameAsync(string naziv, int? excludeId = null);
        Task AddSirovinaAsync(Sirovinazatretman sirovina);
        Task UpdateSirovinaAsync(Sirovinazatretman sirovina);
        Task DeleteSirovinaAsync(Sirovinazatretman sirovina);
    }
}


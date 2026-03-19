using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface IRadoviRepository
    {
        // Radovi CRUD
        Task<List<Radovi>> GetAllRadoviAsync();
        Task<Radovi?> GetRadoviByIdAsync(int id);
        Task<Radovi?> GetRadoviWithDetailsAsync(int id);
        Task<Radovi?> GetRadoviWithParcelaAsync(int id);
        Task<Radovi?> GetRadoviWithRadniciAsync(int id);
        Task AddRadoviAsync(Radovi radovi);
        Task UpdateRadoviAsync(Radovi radovi);
        Task DeleteRadoviAsync(Radovi radovi);

        // Helper methods for relationships
        Task<Parcela?> GetParcelaByIdAsync(int parcelaId);
        Task<Radnik?> GetRadnikByIdAsync(int radnikId);
        Task<Radnik?> GetRadnikWithRadoviAsync(int radnikId);
        Task<List<Radovi>> GetRadoviForRadnikAsync(int radnikId);
        
        // Cross-check validation
        Task<List<Radovi>> GetRadoviForRadnikInDateRangeAsync(int radnikId, DateOnly dateFrom, DateOnly dateTo);
        
        Task SaveChangesAsync();
    }
}


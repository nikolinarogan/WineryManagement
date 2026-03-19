using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface IVinogradRepository
    {
        Task<List<Vinograd>> GetAllVinogradiAsync();
        Task<List<Vinograd>> GetAllVinogradiWithParcelaAsync();
        Task<Vinograd?> GetVinogradByIdAsync(int id);
        Task<Vinograd?> GetVinogradWithParcelaByIdAsync(int id);
        Task<Vinograd> AddVinogradAsync(Vinograd vinograd);
        Task UpdateVinogradAsync(Vinograd vinograd);
        Task DeleteVinogradAsync(Vinograd vinograd);
        Task<Parcela> AddParcelaAsync(Parcela parcela);
        Task<Parcela?> GetParcelaByIdAsync(int id);
        Task UpdateParcelaAsync(Parcela parcela);
        Task DeleteParcelaAsync(Parcela parcela);
        Task SaveChangesAsync();
    }
}


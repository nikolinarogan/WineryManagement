using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface ISortagrozdjaRepository
    {
        Task<List<Sortagrozdja>> GetAllSorteAsync();
        Task<Sortagrozdja?> GetSortaByIdAsync(int id);
        Task<Sortagrozdja?> GetSortaWithParcelaByIdAsync(int id);
        Task<Sortagrozdja> AddSortaAsync(Sortagrozdja sorta);
        Task UpdateSortaAsync(Sortagrozdja sorta);
        Task DeleteSortaAsync(Sortagrozdja sorta);
        Task SaveChangesAsync();
    }
}


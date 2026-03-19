using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class SortagrozdjaRepository : ISortagrozdjaRepository
    {
        private readonly ApplicationDbContext _context;

        public SortagrozdjaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sortagrozdja>> GetAllSorteAsync()
        {
            return await _context.Sortagrozdjas
                .Include(s => s.Parcelas)
                .OrderBy(s => s.Nazivsorte)
                .ToListAsync();
        }

        public async Task<Sortagrozdja?> GetSortaByIdAsync(int id)
        {
            return await _context.Sortagrozdjas.FindAsync(id);
        }

        public async Task<Sortagrozdja?> GetSortaWithParcelaByIdAsync(int id)
        {
            return await _context.Sortagrozdjas
                .Include(s => s.Parcelas)
                    .ThenInclude(p => p.VinogradIdvNavigation)
                .FirstOrDefaultAsync(s => s.Idsrt == id);
        }

        public async Task<Sortagrozdja> AddSortaAsync(Sortagrozdja sorta)
        {
            _context.Sortagrozdjas.Add(sorta);
            await _context.SaveChangesAsync();
            return sorta;
        }

        public async Task UpdateSortaAsync(Sortagrozdja sorta)
        {
            _context.Sortagrozdjas.Update(sorta);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSortaAsync(Sortagrozdja sorta)
        {
            _context.Sortagrozdjas.Remove(sorta);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}


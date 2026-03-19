using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class SirovinazatretmanRepository : ISirovinazatretmanRepository
    {
        private readonly ApplicationDbContext _context;

        public SirovinazatretmanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sirovinazatretman>> GetAllSirovineAsync()
        {
            return await _context.Sirovinazatretmen
                .Include(s => s.SeDodajes)
                .OrderBy(s => s.Naziv)
                .ToListAsync();
        }

        public async Task<Sirovinazatretman?> GetSirovinaByIdAsync(int id)
        {
            return await _context.Sirovinazatretmen.FindAsync(id);
        }

        public async Task<Sirovinazatretman?> GetSirovinaWithUsageAsync(int id)
        {
            return await _context.Sirovinazatretmen
                .Include(s => s.SeDodajes)
                .FirstOrDefaultAsync(s => s.Idsir == id);
        }

        public async Task<bool> SirovinaExistsByNameAsync(string naziv, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Sirovinazatretmen
                    .AnyAsync(s => s.Naziv.ToLower() == naziv.ToLower() && s.Idsir != excludeId.Value);
            }
            else
            {
                return await _context.Sirovinazatretmen
                    .AnyAsync(s => s.Naziv.ToLower() == naziv.ToLower());
            }
        }

        public async Task AddSirovinaAsync(Sirovinazatretman sirovina)
        {
            _context.Sirovinazatretmen.Add(sirovina);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSirovinaAsync(Sirovinazatretman sirovina)
        {
            _context.Sirovinazatretmen.Update(sirovina);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSirovinaAsync(Sirovinazatretman sirovina)
        {
            _context.Sirovinazatretmen.Remove(sirovina);
            await _context.SaveChangesAsync();
        }
    }
}


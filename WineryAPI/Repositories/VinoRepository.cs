using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class VinoRepository : IVinoRepository
    {
        private readonly ApplicationDbContext _context;

        public VinoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vino CRUD
        public async Task<List<Vino>> GetAllVinaAsync()
        {
            return await _context.Vinos
                .Include(v => v.SirovovinoIdsirvinas)
                .OrderBy(v => v.Nazivvina)
                .ToListAsync();
        }

        public async Task<Vino?> GetVinoByIdAsync(int id)
        {
            return await _context.Vinos.FindAsync(id);
        }

        public async Task<Vino?> GetVinoWithSirovimVinimaAsync(int id)
        {
            return await _context.Vinos
                .Include(v => v.SirovovinoIdsirvinas)
                .FirstOrDefaultAsync(v => v.Idvina == id);
        }

        public async Task<Vino?> GetVinoForDeletionAsync(int id)
        {
            return await _context.Vinos
                .Include(v => v.Bocas)                  // Za validaciju boca
                .Include(v => v.Ucestvujes)             // Za validaciju događaja
                .Include(v => v.DegustacijaIddegs)      // Za validaciju degustacija
                .FirstOrDefaultAsync(v => v.Idvina == id);
        }

        public async Task<bool> VinoExistsByNameAsync(string naziv, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Vinos
                    .AnyAsync(v => v.Nazivvina.ToLower() == naziv.ToLower() && v.Idvina != excludeId.Value);
            }
            else
            {
                return await _context.Vinos
                    .AnyAsync(v => v.Nazivvina.ToLower() == naziv.ToLower());
            }
        }

        public async Task AddVinoAsync(Vino vino)
        {
            _context.Vinos.Add(vino);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVinoAsync(Vino vino)
        {
            _context.Vinos.Update(vino);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVinoAsync(Vino vino)
        {
            _context.Vinos.Remove(vino);
            await _context.SaveChangesAsync();
        }

        // Sirovovino queries
        public async Task<List<Sirovovino>> GetSirovaVinaByIdsAsync(List<int> ids)
        {
            return await _context.Sirovovinos
                .Where(sv => ids.Contains(sv.Idsirvina))
                .ToListAsync();
        }
    }
}


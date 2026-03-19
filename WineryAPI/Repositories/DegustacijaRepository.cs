using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class DegustacijaRepository : IDegustacijaRepository
    {
        private readonly ApplicationDbContext _context;

        public DegustacijaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Degustacija>> GetAllDegustacijeAsync()
        {
            return await _context.Degustacijas
                .Include(d => d.VinoIdvinas)
                .Include(d => d.SomleijerIdzaps)
                    .ThenInclude(s => s.IdzapNavigation)
                .OrderByDescending(d => d.Datdeg)
                .ToListAsync();
        }

        public async Task<Degustacija?> GetDegustacijaByIdAsync(int id)
        {
            return await _context.Degustacijas
                .Include(d => d.VinoIdvinas)
                .Include(d => d.SomleijerIdzaps)
                    .ThenInclude(s => s.IdzapNavigation)
                .FirstOrDefaultAsync(d => d.Iddeg == id);
        }

        public async Task<Degustacija> CreateDegustacijaAsync(Degustacija degustacija)
        {
            _context.Degustacijas.Add(degustacija);
            await _context.SaveChangesAsync();
            return degustacija;
        }

        public async Task UpdateDegustacijaAsync(Degustacija degustacija)
        {
            _context.Degustacijas.Update(degustacija);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDegustacijaAsync(int id)
        {
            var degustacija = await _context.Degustacijas.FindAsync(id);
            if (degustacija != null)
            {
                _context.Degustacijas.Remove(degustacija);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Somleijer?> GetSomleijerByIdAsync(int idzap)
        {
            return await _context.Somleijers
                .Include(s => s.IdzapNavigation)
                .FirstOrDefaultAsync(s => s.Idzap == idzap);
        }

        public async Task<bool> DegustacijaExistsAsync(int id)
        {
            return await _context.Degustacijas.AnyAsync(d => d.Iddeg == id);
        }
    }
}


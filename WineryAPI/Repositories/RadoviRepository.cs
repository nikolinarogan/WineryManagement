using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class RadoviRepository : IRadoviRepository
    {
        private readonly ApplicationDbContext _context;

        public RadoviRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Radovi CRUD
        public async Task<List<Radovi>> GetAllRadoviAsync()
        {
            return await _context.Radovis
                .Include(r => r.ParcelaIdps)
                .Include(r => r.RadnikIdzaps)
                .OrderByDescending(r => r.Pocrad)
                .ToListAsync();
        }

        public async Task<Radovi?> GetRadoviByIdAsync(int id)
        {
            return await _context.Radovis.FindAsync(id);
        }

        public async Task<Radovi?> GetRadoviWithDetailsAsync(int id)
        {
            return await _context.Radovis
                .Include(r => r.ParcelaIdps)
                    .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(r => r.ParcelaIdps)
                    .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .Include(r => r.RadnikIdzaps)
                    .ThenInclude(rad => rad.IdzapNavigation)
                .FirstOrDefaultAsync(r => r.Idrad == id);
        }

        public async Task<Radovi?> GetRadoviWithParcelaAsync(int id)
        {
            return await _context.Radovis
                .Include(r => r.ParcelaIdps)
                .FirstOrDefaultAsync(r => r.Idrad == id);
        }

        public async Task<Radovi?> GetRadoviWithRadniciAsync(int id)
        {
            return await _context.Radovis
                .Include(r => r.RadnikIdzaps)
                .FirstOrDefaultAsync(r => r.Idrad == id);
        }

        public async Task AddRadoviAsync(Radovi radovi)
        {
            _context.Radovis.Add(radovi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRadoviAsync(Radovi radovi)
        {
            _context.Radovis.Update(radovi);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRadoviAsync(Radovi radovi)
        {
            _context.Radovis.Remove(radovi);
            await _context.SaveChangesAsync();
        }

        // Helper methods
        public async Task<Parcela?> GetParcelaByIdAsync(int parcelaId)
        {
            return await _context.Parcelas.FindAsync(parcelaId);
        }

        public async Task<Radnik?> GetRadnikByIdAsync(int radnikId)
        {
            return await _context.Radniks.FindAsync(radnikId);
        }

        public async Task<Radnik?> GetRadnikWithRadoviAsync(int radnikId)
        {
            return await _context.Radniks
                .Include(rad => rad.RadoviIdrads)
                    .ThenInclude(r => r.ParcelaIdps)
                        .ThenInclude(p => p.VinogradIdvNavigation)
                .FirstOrDefaultAsync(rad => rad.Idzap == radnikId);
        }

        public async Task<List<Radovi>> GetRadoviForRadnikAsync(int radnikId)
        {
            return await _context.Radovis
                .Include(r => r.ParcelaIdps)
                    .ThenInclude(p => p.VinogradIdvNavigation)
                .Where(r => r.RadnikIdzaps.Any(rad => rad.Idzap == radnikId))
                .ToListAsync();
        }

        public async Task<List<Radovi>> GetRadoviForRadnikInDateRangeAsync(int radnikId, DateOnly dateFrom, DateOnly dateTo)
        {
            return await _context.Radovis
                .Include(r => r.ParcelaIdps)
                    .ThenInclude(p => p.VinogradIdvNavigation)
                .Where(r => r.RadnikIdzaps.Any(rad => rad.Idzap == radnikId))
                .Where(r => r.Pocrad <= dateTo && r.Zavrrad >= dateFrom)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}


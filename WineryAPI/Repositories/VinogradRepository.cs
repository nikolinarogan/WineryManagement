using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class VinogradRepository : IVinogradRepository
    {
        private readonly ApplicationDbContext _context;

        public VinogradRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vinograd>> GetAllVinogradiAsync()
        {
            return await _context.Vinograds
                .Include(v => v.Parcelas)
                .OrderBy(v => v.Naziv)
                .ToListAsync();
        }

        public async Task<List<Vinograd>> GetAllVinogradiWithParcelaAsync()
        {
            return await _context.Vinograds
                .Include(v => v.Parcelas)
                    .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .OrderBy(v => v.Naziv)
                .ToListAsync();
        }

        public async Task<Vinograd?> GetVinogradByIdAsync(int id)
        {
            return await _context.Vinograds.FindAsync(id);
        }

        public async Task<Vinograd?> GetVinogradWithParcelaByIdAsync(int id)
        {
            return await _context.Vinograds
                .Include(v => v.Parcelas)
                    .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .FirstOrDefaultAsync(v => v.Idv == id);
        }

        public async Task<Vinograd> AddVinogradAsync(Vinograd vinograd)
        {
            _context.Vinograds.Add(vinograd);
            await _context.SaveChangesAsync();
            return vinograd;
        }

        public async Task UpdateVinogradAsync(Vinograd vinograd)
        {
            _context.Vinograds.Update(vinograd);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVinogradAsync(Vinograd vinograd)
        {
            _context.Vinograds.Remove(vinograd);
            await _context.SaveChangesAsync();
        }

        public async Task<Parcela> AddParcelaAsync(Parcela parcela)
        {
            _context.Parcelas.Add(parcela);
            await _context.SaveChangesAsync();
            return parcela;
        }

        public async Task<Parcela?> GetParcelaByIdAsync(int id)
        {
            return await _context.Parcelas.FindAsync(id);
        }

        public async Task UpdateParcelaAsync(Parcela parcela)
        {
            _context.Parcelas.Update(parcela);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParcelaAsync(Parcela parcela)
        {
            _context.Parcelas.Remove(parcela);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}


using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class UbranasirovinaRepository : IUbranasirovinaRepository
    {
        private readonly ApplicationDbContext _context;

        public UbranasirovinaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ubranasirovina CRUD
        public async Task<List<Ubranasirovina>> GetAllUbranasirovinaAsync()
        {
            return await _context.Ubranasirovinas
                .Include(u => u.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.BerbaIdberNavigation)
                .Include(u => u.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.ParcelaIdpNavigation)
                            .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(u => u.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.ParcelaIdpNavigation)
                            .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .OrderByDescending(u => u.Datprijema)
                .ToListAsync();
        }

        public async Task<Ubranasirovina?> GetUbranasirovinaByIdAsync(int id)
        {
            return await _context.Ubranasirovinas.FindAsync(id);
        }

        public async Task<Ubranasirovina?> GetUbranasirovinaWithDetailsAsync(int id)
        {
            return await _context.Ubranasirovinas
                .Include(u => u.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.BerbaIdberNavigation)
                .Include(u => u.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.ParcelaIdpNavigation)
                            .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(u => u.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.ParcelaIdpNavigation)
                            .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .FirstOrDefaultAsync(u => u.Idubrsir == id);
        }

        public async Task AddUbranasirovinaAsync(Ubranasirovina ubranasirovina)
        {
            _context.Ubranasirovinas.Add(ubranasirovina);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUbranasirovinaAsync(Ubranasirovina ubranasirovina)
        {
            _context.Ubranasirovinas.Update(ubranasirovina);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUbranasirovinaAsync(Ubranasirovina ubranasirovina)
        {
            _context.Ubranasirovinas.Remove(ubranasirovina);
            await _context.SaveChangesAsync();
        }

        // Rasporedbranja queries
        public async Task<List<Rasporedbranja>> GetRasporedbranjaForPrijemAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await _context.Rasporedbranjas
                .Include(r => r.JeAngazovans)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.BerbaIdberNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                        .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                        .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .Include(r => r.Ubranasirovina)
                .Where(r =>
                    r.Ubranasirovina == null && // Još nije primljen
                    r.JeAngazovans.Any() && // Ima dodijeljenih radnika
                    (r.Zavrsetakbranja < today || // Datum istekao ILI
                     r.JeAngazovans.All(ja => ja.Kolicinaubrgr > 0)) // Svi unijeli količinu > 0
                )
                .ToListAsync();
        }

        public async Task<Rasporedbranja?> GetRasporedbranjaWithFullDetailsAsync(int rasporedId)
        {
            return await _context.Rasporedbranjas
                .Include(r => r.JeAngazovans)
                    .ThenInclude(ja => ja.RadnikIdzapNavigation)
                        .ThenInclude(rad => rad.IdzapNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.BerbaIdberNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                        .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                        .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .FirstOrDefaultAsync(r => r.Idras == rasporedId);
        }

        public async Task<Rasporedbranja?> GetRasporedbranjaForValidationAsync(int rasporedId)
        {
            return await _context.Rasporedbranjas
                .Include(r => r.JeAngazovans)
                .Include(r => r.Ubranasirovina)
                .FirstOrDefaultAsync(r => r.Idras == rasporedId);
        }
    }
}


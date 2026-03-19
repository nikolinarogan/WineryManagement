using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class TretmanRepository : ITretmanRepository
    {
        private readonly ApplicationDbContext _context;

        public TretmanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ubranasirovina queries
        public async Task<List<Ubranasirovina>> GetAllUbraneSirovineAsync()
        {
            return await _context.Ubranasirovinas
                .Include(us => us.RasporedbranjaIdrasNavigation)
                    .ThenInclude(rb => rb.Seodrzava)
                        .ThenInclude(so => so.BerbaIdberNavigation)
                .Include(us => us.RasporedbranjaIdrasNavigation)
                    .ThenInclude(rb => rb.Seodrzava)
                        .ThenInclude(so => so.ParcelaIdpNavigation)
                            .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(us => us.RasporedbranjaIdrasNavigation)
                    .ThenInclude(rb => rb.Seodrzava)
                        .ThenInclude(so => so.ParcelaIdpNavigation)
                            .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .Include(us => us.Tretmen)
                .Include(us => us.JeOsnovas) // je_osnova veza (AŽURIRANO)
                .ToListAsync();
        }

        public async Task<Ubranasirovina?> GetUbranaSirovinaByIdAsync(int id)
        {
            return await _context.Ubranasirovinas.FindAsync(id);
        }

        public async Task<Ubranasirovina?> GetUbranaSirovinaWithDetailsAsync(int id)
        {
            return await _context.Ubranasirovinas
                .Include(us => us.RasporedbranjaIdrasNavigation)
                    .ThenInclude(rb => rb.Seodrzava)
                        .ThenInclude(so => so.BerbaIdberNavigation)
                .Include(us => us.RasporedbranjaIdrasNavigation)
                    .ThenInclude(rb => rb.Seodrzava)
                        .ThenInclude(so => so.ParcelaIdpNavigation)
                            .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(us => us.RasporedbranjaIdrasNavigation)
                    .ThenInclude(rb => rb.Seodrzava)
                        .ThenInclude(so => so.ParcelaIdpNavigation)
                            .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .Include(us => us.Tretmen)
                .Include(us => us.JeOsnovas) // je_osnova veza (AŽURIRANO)
                .FirstOrDefaultAsync(us => us.Idubrsir == id);
        }

        // Tretman CRUD
        public async Task<List<Tretman>> GetTretmaniByUbranaSirovinaAsync(int ubranasirovinaId)
        {
            return await _context.Tretmen
                .Where(t => t.UbranasirovinaIdubrsir == ubranasirovinaId)
                .Include(t => t.SeDodajes)
                .Include(t => t.EnologIdzaps)
                    .ThenInclude(e => e.IdzapNavigation)
                .OrderBy(t => t.Datpocetkatret)
                .ToListAsync();
        }

        public async Task<Tretman?> GetTretmanByIdAsync(int id)
        {
            return await _context.Tretmen.FindAsync(id);
        }

        public async Task<Tretman?> GetTretmanWithDetailsAsync(int id)
        {
            return await _context.Tretmen
                .Include(t => t.UbranasirovinaIdubrsirNavigation)
                    .ThenInclude(us => us.RasporedbranjaIdrasNavigation)
                        .ThenInclude(rb => rb.Seodrzava)
                            .ThenInclude(so => so.BerbaIdberNavigation)
                .Include(t => t.UbranasirovinaIdubrsirNavigation)
                    .ThenInclude(us => us.RasporedbranjaIdrasNavigation)
                        .ThenInclude(rb => rb.Seodrzava)
                            .ThenInclude(so => so.ParcelaIdpNavigation)
                                .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .Include(t => t.SeDodajes)
                    .ThenInclude(sd => sd.SirovinazatretmanIdsirNavigation)
                .Include(t => t.EnologIdzaps)
                    .ThenInclude(e => e.IdzapNavigation)
                .FirstOrDefaultAsync(t => t.Idtretmana == id);
        }

        public async Task<Tretman?> GetTretmanWithSirovineAsync(int id)
        {
            return await _context.Tretmen
                .Include(t => t.SeDodajes)
                .FirstOrDefaultAsync(t => t.Idtretmana == id);
        }

        public async Task AddTretmanAsync(Tretman tretman)
        {
            _context.Tretmen.Add(tretman);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTretmanAsync(Tretman tretman)
        {
            _context.Tretmen.Update(tretman);
            await _context.SaveChangesAsync();
        }

        // Tretman filter queries
        public async Task<List<Tretman>> GetAktivniTretmaniAsync()
        {
            return await _context.Tretmen
                .Where(t => t.Datzavresetkatret == null)
                .Include(t => t.SeDodajes)
                .Include(t => t.EnologIdzaps)
                    .ThenInclude(e => e.IdzapNavigation)
                .OrderBy(t => t.Datpocetkatret)
                .ToListAsync();
        }

        public async Task<List<Tretman>> GetZavreniTretmaniAsync()
        {
            return await _context.Tretmen
                .Where(t => t.Datzavresetkatret != null)
                .Include(t => t.SeDodajes)
                .Include(t => t.EnologIdzaps)
                    .ThenInclude(e => e.IdzapNavigation)
                .OrderByDescending(t => t.Datzavresetkatret)
                .ToListAsync();
        }

        // Enolog queries
        public async Task<Enolog?> GetEnologByIdAsync(int id)
        {
            return await _context.Enologs.FindAsync(id);
        }

        // Sirovinazatretman queries
        public async Task<Sirovinazatretman?> GetSirovinazatretmanByIdAsync(int id)
        {
            return await _context.Sirovinazatretmen.FindAsync(id);
        }

        // SeDodaje CRUD
        public async Task AddSeDodajeAsync(SeDodaje seDodaje)
        {
            _context.SeDodajes.Add(seDodaje);
            await _context.SaveChangesAsync();
        }

        public async Task<SeDodaje?> GetSeDodajeAsync(int tretmanId, int sirovinazatretmanId)
        {
            return await _context.SeDodajes
                .FirstOrDefaultAsync(sd => sd.TretmanIdtretmana == tretmanId 
                                         && sd.SirovinazatretmanIdsir == sirovinazatretmanId);
        }

        public async Task DeleteSeDodajeAsync(SeDodaje seDodaje)
        {
            _context.SeDodajes.Remove(seDodaje);
            await _context.SaveChangesAsync();
        }
    }
}


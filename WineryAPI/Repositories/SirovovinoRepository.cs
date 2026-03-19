using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class SirovovinoRepository : ISirovovinoRepository
    {
        private readonly ApplicationDbContext _context;

        public SirovovinoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sirovovino CRUD
        public async Task<List<Sirovovino>> GetAllSirovovinaAsync()
        {
            return await _context.Sirovovinos
                .Include(sv => sv.JeOsnovas)
                    .ThenInclude(jo => jo.UbranasirovinaIdubrsirNavigation)
                        .ThenInclude(us => us.RasporedbranjaIdrasNavigation)
                            .ThenInclude(rb => rb.Seodrzava)
                                .ThenInclude(so => so.BerbaIdberNavigation)
                .Include(sv => sv.JeOsnovas)
                    .ThenInclude(jo => jo.UbranasirovinaIdubrsirNavigation)
                        .ThenInclude(us => us.RasporedbranjaIdrasNavigation)
                            .ThenInclude(rb => rb.Seodrzava)
                                .ThenInclude(so => so.ParcelaIdpNavigation)
                                    .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .Include(sv => sv.JeOsnovas)
                    .ThenInclude(jo => jo.UbranasirovinaIdubrsirNavigation)
                        .ThenInclude(us => us.Tretmen)
                .OrderByDescending(sv => sv.Godproizvodnje)
                .ThenBy(sv => sv.Nazivsirvina)
                .ToListAsync();
        }

        public async Task<Sirovovino?> GetSirovovinoByIdAsync(int id)
        {
            return await _context.Sirovovinos.FindAsync(id);
        }

        public async Task<Sirovovino?> GetSirovovinoWithDetailsAsync(int id)
        {
            return await _context.Sirovovinos
                .Include(sv => sv.JeOsnovas)
                    .ThenInclude(jo => jo.UbranasirovinaIdubrsirNavigation)
                        .ThenInclude(us => us.RasporedbranjaIdrasNavigation)
                            .ThenInclude(rb => rb.Seodrzava)
                                .ThenInclude(so => so.BerbaIdberNavigation)
                .Include(sv => sv.JeOsnovas)
                    .ThenInclude(jo => jo.UbranasirovinaIdubrsirNavigation)
                        .ThenInclude(us => us.RasporedbranjaIdrasNavigation)
                            .ThenInclude(rb => rb.Seodrzava)
                                .ThenInclude(so => so.ParcelaIdpNavigation)
                                    .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .Include(sv => sv.JeOsnovas)
                    .ThenInclude(jo => jo.UbranasirovinaIdubrsirNavigation)
                        .ThenInclude(us => us.Tretmen)
                .FirstOrDefaultAsync(sv => sv.Idsirvina == id);
        }

        public async Task<Sirovovino?> GetSirovovinoForDeletionAsync(int id)
        {
            return await _context.Sirovovinos
                .Include(sv => sv.SeLagerujes)          // Za validaciju lagerovanja
                .Include(sv => sv.VinoIdvinas)          // Za validaciju blendinga
                .FirstOrDefaultAsync(sv => sv.Idsirvina == id);
        }

        public async Task AddSirovovinoAsync(Sirovovino sirovovino)
        {
            _context.Sirovovinos.Add(sirovovino);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSirovovinoAsync(Sirovovino sirovovino)
        {
            _context.Sirovovinos.Update(sirovovino);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSirovovinoAsync(Sirovovino sirovovino)
        {
            _context.Sirovovinos.Remove(sirovovino);
            await _context.SaveChangesAsync();
        }

        // Ubranasirovina queries
        public async Task<Ubranasirovina?> GetUbranaSirovinaForValidationAsync(int id)
        {
            return await _context.Ubranasirovinas
                .Include(us => us.Tretmen)                  // Za validaciju tretmana
                .Include(us => us.JeOsnovas)                // Za validaciju duplikata (AŽURIRANO)
                .FirstOrDefaultAsync(us => us.Idubrsir == id);
        }

        // Validacija
        public async Task<bool> SirovoVinoExistsForUbranaSirovinaAsync(int ubranasirovinaId)
        {
            return await _context.JeOsnovas
                .AnyAsync(jo => jo.UbranasirovinaIdubrsir == ubranasirovinaId);
        }

        // JeOsnova queries (za praćenje iskorišćenosti grožđa)
        public async Task<decimal> GetIskoriscenoGrozdje(int ubranasirovinaId)
        {
            var iskorisceno = await _context.JeOsnovas
                .Where(jo => jo.UbranasirovinaIdubrsir == ubranasirovinaId)
                .SumAsync(jo => jo.KolicinaGrozdja);

            return iskorisceno;
        }

        // JeOsnova CRUD
        public async Task AddJeOsnovaAsync(JeOsnova jeOsnova)
        {
            _context.JeOsnovas.Add(jeOsnova);
            await _context.SaveChangesAsync();
        }
    }
}


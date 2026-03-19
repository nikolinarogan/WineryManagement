using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class BerbaRepository : IBerbaRepository
    {
        private readonly ApplicationDbContext _context;

        public BerbaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Berba CRUD
        public async Task<List<Berba>> GetAllBerbeAsync()
        {
            return await _context.Berbas
                .Include(b => b.Seodrzavas)
                .OrderByDescending(b => b.Sezona)
                .ThenBy(b => b.Nazber)
                .ToListAsync();
        }

        public async Task<Berba?> GetBerbaByIdAsync(int id)
        {
            return await _context.Berbas.FindAsync(id);
        }

        public async Task<Berba?> GetBerbaWithDetailsAsync(int id)
        {
            return await _context.Berbas
                .Include(b => b.Seodrzavas)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                        .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(b => b.Seodrzavas)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                        .ThenInclude(p => p.SortagrozdjaIdsrtNavigation)
                .FirstOrDefaultAsync(b => b.Idber == id);
        }

        public async Task<Berba> AddBerbaAsync(Berba berba)
        {
            _context.Berbas.Add(berba);
            await _context.SaveChangesAsync();
            return berba;
        }

        public async Task UpdateBerbaAsync(Berba berba)
        {
            _context.Berbas.Update(berba);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBerbaAsync(Berba berba)
        {
            _context.Berbas.Remove(berba);
            await _context.SaveChangesAsync();
        }

        // Seodrzava (Berba-Parcela relationship)
        public async Task<Seodrzava?> GetSeodrzavaAsync(int berbaId, int parcelaId)
        {
            return await _context.Seodrzavas
                .FirstOrDefaultAsync(s => s.BerbaIdber == berbaId && s.ParcelaIdp == parcelaId);
        }

        public async Task AddSeodrzavaAsync(Seodrzava seodrzava)
        {
            _context.Seodrzavas.Add(seodrzava);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSeodrzavaAsync(Seodrzava seodrzava)
        {
            _context.Seodrzavas.Remove(seodrzava);
            await _context.SaveChangesAsync();
        }

        // RasporedBranja CRUD
        public async Task<List<Rasporedbranja>> GetAllRasporedbranjaAsync(int? berbaId = null)
        {
            var query = _context.Rasporedbranjas
                .Include(r => r.MenadzerIdzapNavigation)
                    .ThenInclude(m => m.IdzapNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.BerbaIdberNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                .Include(r => r.JeAngazovans)
                .AsQueryable();

            if (berbaId.HasValue)
            {
                query = query.Where(r => r.SeodrzavaIdber == berbaId.Value);
            }

            return await query
                .OrderByDescending(r => r.Pocbranja)
                .ToListAsync();
        }

        public async Task<Rasporedbranja?> GetRasporedbranjaByIdAsync(int id)
        {
            return await _context.Rasporedbranjas.FindAsync(id);
        }

        public async Task<Rasporedbranja?> GetRasporedbranjaWithDetailsAsync(int id)
        {
            return await _context.Rasporedbranjas
                .Include(r => r.MenadzerIdzapNavigation)
                    .ThenInclude(m => m.IdzapNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.BerbaIdberNavigation)
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.ParcelaIdpNavigation)
                        .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(r => r.JeAngazovans)
                    .ThenInclude(ja => ja.RadnikIdzapNavigation)
                        .ThenInclude(rad => rad.IdzapNavigation)
                .FirstOrDefaultAsync(r => r.Idras == id);
        }

        public async Task<Rasporedbranja> AddRasporedbranjaAsync(Rasporedbranja raspored)
        {
            _context.Rasporedbranjas.Add(raspored);
            await _context.SaveChangesAsync();
            return raspored;
        }

        public async Task UpdateRasporedbranjaAsync(Rasporedbranja raspored)
        {
            _context.Rasporedbranjas.Update(raspored);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRasporedbranjaAsync(Rasporedbranja raspored)
        {
            _context.Rasporedbranjas.Remove(raspored);
            await _context.SaveChangesAsync();
        }

        // JeAngazovan (RasporedBranja-Radnik relationship)
        public async Task<JeAngazovan?> GetJeAngazovanAsync(int rasporedId, int radnikId)
        {
            return await _context.JeAngazovans
                .FirstOrDefaultAsync(j => j.RasporedbranjaIdras == rasporedId && j.RadnikIdzap == radnikId);
        }

        public async Task<List<JeAngazovan>> GetJeAngazovanByRasporedAsync(int rasporedId)
        {
            return await _context.JeAngazovans
                .Include(j => j.RadnikIdzapNavigation)
                .Where(j => j.RasporedbranjaIdras == rasporedId)
                .ToListAsync();
        }

        public async Task<List<JeAngazovan>> GetJeAngazovanByRadnikAsync(int radnikId)
        {
            return await _context.JeAngazovans
                .Include(j => j.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.BerbaIdberNavigation)
                .Include(j => j.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.Seodrzava)
                        .ThenInclude(s => s.ParcelaIdpNavigation)
                            .ThenInclude(p => p.VinogradIdvNavigation)
                .Include(j => j.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.MenadzerIdzapNavigation)
                        .ThenInclude(m => m.IdzapNavigation)
                .Include(j => j.RasporedbranjaIdrasNavigation)
                    .ThenInclude(r => r.JeAngazovans)
                .Include(j => j.RadnikIdzapNavigation)
                .Where(j => j.RadnikIdzap == radnikId)
                .ToListAsync();
        }

        public async Task AddJeAngazovanAsync(JeAngazovan jeAngazovan)
        {
            _context.JeAngazovans.Add(jeAngazovan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateJeAngazovanAsync(JeAngazovan jeAngazovan)
        {
            _context.JeAngazovans.Update(jeAngazovan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteJeAngazovanAsync(JeAngazovan jeAngazovan)
        {
            _context.JeAngazovans.Remove(jeAngazovan);
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

        public async Task<List<Rasporedbranja>> GetRasporedbranjaByBerbaIdAsync(int berbaId)
        {
            return await _context.Rasporedbranjas
                .Include(r => r.JeAngazovans)
                    .ThenInclude(ja => ja.RadnikIdzapNavigation)
                        .ThenInclude(rad => rad.IdzapNavigation)
                .Where(r => r.SeodrzavaIdber == berbaId)
                .ToListAsync();
        }

        public async Task<List<Rasporedbranja>> GetRasporedbranjaForRadnikInDateRangeAsync(int radnikId, DateOnly dateFrom, DateOnly dateTo)
        {
            return await _context.Rasporedbranjas
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.BerbaIdberNavigation) // Berba
                .Include(r => r.Seodrzava)
                    .ThenInclude(s => s.ParcelaIdpNavigation)   // Parcela
                .Where(r => r.JeAngazovans.Any(ja => ja.RadnikIdzap == radnikId))
                .Where(r => r.Pocbranja <= dateTo && r.Zavrsetakbranja >= dateFrom)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}


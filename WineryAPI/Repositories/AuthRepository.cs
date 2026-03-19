using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Zaposleni?> GetZaposleniByEmailAsync(string email)
        {
            return await _context.Zaposlenis
                .FirstOrDefaultAsync(z => z.Email == email);
        }

        public async Task<Zaposleni?> GetZaposleniByIdAsync(int id)
        {
            return await _context.Zaposlenis.FindAsync(id);
        }

        public async Task<Zaposleni?> GetZaposleniWithDetailsAsync(int id)
        {
            return await _context.Zaposlenis
                .Include(z => z.Enolog)
                .Include(z => z.Menadzer)
                .Include(z => z.Radnik)
                .Include(z => z.Somleijer)
                .FirstOrDefaultAsync(z => z.Idzap == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Zaposlenis.AnyAsync(z => z.Email == email);
        }

        public async Task<bool> JmbgExistsAsync(string jmbg)
        {
            return await _context.Zaposlenis.AnyAsync(z => z.Jmbg == jmbg);
        }

        public async Task<Zaposleni> AddZaposleniAsync(Zaposleni zaposleni)
        {
            _context.Zaposlenis.Add(zaposleni);
            await _context.SaveChangesAsync();
            return zaposleni;
        }

        public async Task AddEnologAsync(Enolog enolog)
        {
            _context.Enologs.Add(enolog);
            await _context.SaveChangesAsync();
        }

        public async Task AddRadnikAsync(Radnik radnik)
        {
            _context.Radniks.Add(radnik);
            await _context.SaveChangesAsync();
        }

        public async Task AddSomleijerAsync(Somleijer somleijer)
        {
            _context.Somleijers.Add(somleijer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateZaposleniAsync(Zaposleni zaposleni)
        {
            _context.Zaposlenis.Update(zaposleni);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Zaposleni>> GetAllZaposleniAsync(string? kategorija = null)
        {
            var query = _context.Zaposlenis.AsQueryable();

            if (!string.IsNullOrEmpty(kategorija))
            {
                query = query.Where(z => z.Kategorija == kategorija);
            }

            return await query
                .OrderBy(z => z.Prez)
                .ThenBy(z => z.Ime)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}


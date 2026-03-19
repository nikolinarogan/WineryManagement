using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;

namespace WineryAPI.Services
{
    public class MagacinService : IMagacinService
    {
        private readonly ApplicationDbContext _context;

        public MagacinService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MagacinDto>> GetAllMagaciniAsync()
        {
            return await _context.Magacins
                .Include(m => m.Bocas)
                .Select(m => new MagacinDto
                {
                    Idmag = m.Idmag,
                    Nazivmag = m.Nazivmag,
                    Kapacitetmag = m.Kapacitetmag,
                    Tempmag = m.Tempmag,
                    BrojBoca = m.Bocas.Count,
                    Popunjenost = m.Kapacitetmag > 0 ? (int)((m.Bocas.Count * 100.0) / m.Kapacitetmag) : 0
                })
                .OrderBy(m => m.Nazivmag)
                .ToListAsync();
        }

        public async Task<MagacinDetailDto?> GetMagacinByIdAsync(int id)
        {
            var magacin = await _context.Magacins
                .Include(m => m.Bocas)
                .Where(m => m.Idmag == id)
                .Select(m => new MagacinDetailDto
                {
                    Idmag = m.Idmag,
                    Nazivmag = m.Nazivmag,
                    Kapacitetmag = m.Kapacitetmag,
                    Tempmag = m.Tempmag,
                    BrojBoca = m.Bocas.Count,
                    Popunjenost = m.Kapacitetmag > 0 ? (int)((m.Bocas.Count * 100.0) / m.Kapacitetmag) : 0
                })
                .FirstOrDefaultAsync();

            return magacin;
        }

        public async Task<MagacinDto> CreateMagacinAsync(CreateMagacinDto dto)
        {
            var postojiNaziv = await _context.Magacins
                .AnyAsync(m => m.Nazivmag.ToLower() == dto.Nazivmag.ToLower());

            if (postojiNaziv)
            {
                throw new InvalidOperationException($"Magacin sa nazivom '{dto.Nazivmag}' već postoji.");
            }

            if (dto.Kapacitetmag <= 0)
            {
                throw new InvalidOperationException("Kapacitet magacina mora biti veći od 0.");
            }

            if (dto.Tempmag < -5 || dto.Tempmag > 30)
            {
                throw new InvalidOperationException("Temperatura magacina mora biti između -5°C i 30°C.");
            }

            var magacin = new Magacin
            {
                Nazivmag = dto.Nazivmag,
                Kapacitetmag = dto.Kapacitetmag,
                Tempmag = dto.Tempmag
            };

            _context.Magacins.Add(magacin);
            await _context.SaveChangesAsync();

            return new MagacinDto
            {
                Idmag = magacin.Idmag,
                Nazivmag = magacin.Nazivmag,
                Kapacitetmag = magacin.Kapacitetmag,
                Tempmag = magacin.Tempmag,
                BrojBoca = 0,
                Popunjenost = 0
            };
        }

        public async Task UpdateMagacinAsync(int id, UpdateMagacinDto dto)
        {
            var magacin = await _context.Magacins.FindAsync(id);

            if (magacin == null)
            {
                throw new KeyNotFoundException($"Magacin sa ID {id} nije pronađen.");
            }

            var postojiNaziv = await _context.Magacins
                .AnyAsync(m => m.Nazivmag.ToLower() == dto.Nazivmag.ToLower() && m.Idmag != id);

            if (postojiNaziv)
            {
                throw new InvalidOperationException($"Magacin sa nazivom '{dto.Nazivmag}' već postoji.");
            }

            if (dto.Kapacitetmag <= 0)
            {
                throw new InvalidOperationException("Kapacitet magacina mora biti veći od 0.");
            }

            if (dto.Tempmag < -5 || dto.Tempmag > 30)
            {
                throw new InvalidOperationException("Temperatura magacina mora biti između -5°C i 30°C.");
            }

            magacin.Nazivmag = dto.Nazivmag;
            magacin.Kapacitetmag = dto.Kapacitetmag;
            magacin.Tempmag = dto.Tempmag;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMagacinAsync(int id)
        {
            var magacin = await _context.Magacins
                .Include(m => m.Bocas)
                .FirstOrDefaultAsync(m => m.Idmag == id);

            if (magacin == null)
            {
                throw new KeyNotFoundException($"Magacin sa ID {id} nije pronađen.");
            }

            if (magacin.Bocas.Any())
            {
                throw new InvalidOperationException(
                    $"Magacin '{magacin.Nazivmag}' ne može biti obrisan jer sadrži {magacin.Bocas.Count} boca vina.");
            }

            _context.Magacins.Remove(magacin);
            await _context.SaveChangesAsync();
        }
    }
}


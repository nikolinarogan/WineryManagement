using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;

namespace WineryAPI.Services
{
    public class PodrumService : IPodrumService
    {
        private readonly ApplicationDbContext _context;

        public PodrumService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PodrumDto>> GetAllPodrumiAsync()
        {
            return await _context.Podrums
                .Include(p => p.Bures)
                .Select(p => new PodrumDto
                {
                    Idpod = p.Idpod,
                    Temp = p.Temp,
                    Nazivpod = p.Nazivpod,
                    BrojBuradi = p.Bures.Count
                })
                .OrderBy(p => p.Nazivpod)
                .ToListAsync();
        }

        public async Task<PodrumDetailDto?> GetPodrumByIdAsync(int id)
        {
            var podrum = await _context.Podrums
                .Include(p => p.Bures)
                .Where(p => p.Idpod == id)
                .Select(p => new PodrumDetailDto
                {
                    Idpod = p.Idpod,
                    Temp = p.Temp,
                    Nazivpod = p.Nazivpod,
                    BrojBuradi = p.Bures.Count
                })
                .FirstOrDefaultAsync();

            return podrum;
        }

        public async Task<PodrumDto> CreatePodrumAsync(CreatePodrumDto dto)
        {
            var postojiNaziv = await _context.Podrums
                .AnyAsync(p => p.Nazivpod.ToLower() == dto.Nazivpod.ToLower());

            if (postojiNaziv)
            {
                throw new InvalidOperationException($"Podrum sa nazivom '{dto.Nazivpod}' već postoji.");
            }

            if (dto.Temp < -5 || dto.Temp > 30)
            {
                throw new InvalidOperationException("Temperatura podruma mora biti između -5°C i 30°C.");
            }

            var podrum = new Podrum
            {
                Temp = dto.Temp,
                Nazivpod = dto.Nazivpod
            };

            _context.Podrums.Add(podrum);
            await _context.SaveChangesAsync();

            return new PodrumDto
            {
                Idpod = podrum.Idpod,
                Temp = podrum.Temp,
                Nazivpod = podrum.Nazivpod,
                BrojBuradi = 0
            };
        }

        public async Task UpdatePodrumAsync(int id, UpdatePodrumDto dto)
        {
            var podrum = await _context.Podrums.FindAsync(id);

            if (podrum == null)
            {
                throw new KeyNotFoundException($"Podrum sa ID {id} nije pronađen.");
            }

            var postojiNaziv = await _context.Podrums
                .AnyAsync(p => p.Nazivpod.ToLower() == dto.Nazivpod.ToLower() && p.Idpod != id);

            if (postojiNaziv)
            {
                throw new InvalidOperationException($"Podrum sa nazivom '{dto.Nazivpod}' već postoji.");
            }

            if (dto.Temp < -5 || dto.Temp > 30)
            {
                throw new InvalidOperationException("Temperatura podruma mora biti između -5°C i 30°C.");
            }

            podrum.Temp = dto.Temp;
            podrum.Nazivpod = dto.Nazivpod;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePodrumAsync(int id)
        {
            var podrum = await _context.Podrums
                .Include(p => p.Bures)
                .FirstOrDefaultAsync(p => p.Idpod == id);

            if (podrum == null)
            {
                throw new KeyNotFoundException($"Podrum sa ID {id} nije pronađen.");
            }

            if (podrum.Bures.Any())
            {
                throw new InvalidOperationException(
                    $"Podrum '{podrum.Nazivpod}' ne može biti obrisan jer sadrži {podrum.Bures.Count} buradi.");
            }

            _context.Podrums.Remove(podrum);
            await _context.SaveChangesAsync();
        }
    }
}


using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;

namespace WineryAPI.Services
{
    public class BureService : IBureService
    {
        private readonly ApplicationDbContext _context;

        public BureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BureDto>> GetBuradiByPodrumIdAsync(int podrumId)
        {
            return await _context.Bures
                .Include(b => b.PodrumIdpodNavigation)
                .Where(b => b.PodrumIdpod == podrumId)
                .Select(b => new BureDto
                {
                    Idbur = b.Idbur,
                    Zapremina = b.Zapremina,
                    Materijal = b.Materijal,
                    Oznakabur = b.Oznakabur,
                    PodrumIdpod = b.PodrumIdpod,
                    PodrumNaziv = b.PodrumIdpodNavigation != null ? b.PodrumIdpodNavigation.Nazivpod : null
                })
                .OrderBy(b => b.Oznakabur)
                .ToListAsync();
        }

        public async Task<BureDetailDto?> GetBureByIdAsync(int id)
        {
            var bure = await _context.Bures
                .Include(b => b.PodrumIdpodNavigation)
                .Include(b => b.SeLagerujes)
                .Where(b => b.Idbur == id)
                .Select(b => new BureDetailDto
                {
                    Idbur = b.Idbur,
                    Zapremina = b.Zapremina,
                    Materijal = b.Materijal,
                    Oznakabur = b.Oznakabur,
                    PodrumIdpod = b.PodrumIdpod,
                    PodrumNaziv = b.PodrumIdpodNavigation != null ? b.PodrumIdpodNavigation.Nazivpod : null,
                    BrojLagerovanihVina = b.SeLagerujes.Count
                })
                .FirstOrDefaultAsync();

            return bure;
        }

        public async Task<BureDto> CreateBureAsync(CreateBureDto dto)
        {
            var podrum = await _context.Podrums.FindAsync(dto.PodrumIdpod);
            if (podrum == null)
            {
                throw new KeyNotFoundException($"Podrum sa ID {dto.PodrumIdpod} nije pronađen.");
            }

            var postojiOznaka = await _context.Bures
                .AnyAsync(b => b.Oznakabur.ToLower() == dto.Oznakabur.ToLower() 
                            && b.PodrumIdpod == dto.PodrumIdpod);

            if (postojiOznaka)
            {
                throw new InvalidOperationException($"Bure sa oznakom '{dto.Oznakabur}' već postoji u podrumu '{podrum.Nazivpod}'.");
            }

            if (dto.Zapremina <= 0)
            {
                throw new InvalidOperationException("Zapremina bureta mora biti veća od 0.");
            }

            var bure = new Bure
            {
                Zapremina = dto.Zapremina,
                Materijal = dto.Materijal,
                Oznakabur = dto.Oznakabur,
                PodrumIdpod = dto.PodrumIdpod
            };

            _context.Bures.Add(bure);
            await _context.SaveChangesAsync();

            return new BureDto
            {
                Idbur = bure.Idbur,
                Zapremina = bure.Zapremina,
                Materijal = bure.Materijal,
                Oznakabur = bure.Oznakabur,
                PodrumIdpod = bure.PodrumIdpod,
                PodrumNaziv = podrum.Nazivpod
            };
        }

        public async Task UpdateBureAsync(int id, UpdateBureDto dto)
        {
            var bure = await _context.Bures
                .Include(b => b.PodrumIdpodNavigation)
                .FirstOrDefaultAsync(b => b.Idbur == id);

            if (bure == null)
            {
                throw new KeyNotFoundException($"Bure sa ID {id} nije pronađeno.");
            }

            var postojiOznaka = await _context.Bures
                .AnyAsync(b => b.Oznakabur.ToLower() == dto.Oznakabur.ToLower() 
                            && b.PodrumIdpod == bure.PodrumIdpod 
                            && b.Idbur != id);

            if (postojiOznaka)
            {
                throw new InvalidOperationException($"Bure sa oznakom '{dto.Oznakabur}' već postoji u podrumu '{bure.PodrumIdpodNavigation?.Nazivpod}'.");
            }

            if (dto.Zapremina <= 0)
            {
                throw new InvalidOperationException("Zapremina bureta mora biti veća od 0.");
            }

            bure.Zapremina = dto.Zapremina;
            bure.Materijal = dto.Materijal;
            bure.Oznakabur = dto.Oznakabur;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBureAsync(int id)
        {
            var bure = await _context.Bures
                .Include(b => b.SeLagerujes)
                .FirstOrDefaultAsync(b => b.Idbur == id);

            if (bure == null)
            {
                throw new KeyNotFoundException($"Bure sa ID {id} nije pronađeno.");
            }

            if (bure.SeLagerujes.Any())
            {
                throw new InvalidOperationException(
                    $"Bure '{bure.Oznakabur}' ne može biti obrisano jer sadrži lagerovano vino.");
            }

           

            _context.Bures.Remove(bure);
            await _context.SaveChangesAsync();
        }
    }
}


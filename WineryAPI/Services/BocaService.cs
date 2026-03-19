using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;

namespace WineryAPI.Services
{
    public class BocaService : IBocaService
    {
        private readonly ApplicationDbContext _context;

        public BocaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BocePunjenjeResultDto> CreateBocePunjenjeAsync(CreateBocaPunjenjeDto dto)
        {
            var vino = await _context.Vinos.FindAsync(dto.VinoIdvina);
            if (vino == null)
            {
                throw new KeyNotFoundException($"Finalno vino sa ID {dto.VinoIdvina} nije pronađeno.");
            }

            var magacin = await _context.Magacins
                .Include(m => m.Bocas)
                .FirstOrDefaultAsync(m => m.Idmag == dto.MagacinIdmag);

            if (magacin == null)
            {
                throw new KeyNotFoundException($"Magacin sa ID {dto.MagacinIdmag} nije pronađen.");
            }

            var trenutniBrojBoca = magacin.Bocas.Count;
            if (trenutniBrojBoca + dto.BrojBoca > magacin.Kapacitetmag)
            {
                throw new InvalidOperationException(
                    $"Magacin '{magacin.Nazivmag}' nema dovoljno kapaciteta. " +
                    $"Trenutno: {trenutniBrojBoca}/{magacin.Kapacitetmag}, " +
                    $"Pokušaj dodavanja: {dto.BrojBoca}");
            }

            if (dto.BrojBoca <= 0)
            {
                throw new InvalidOperationException("Broj boca mora biti veći od 0.");
            }

            if (dto.Zapremina <= 0)
            {
                throw new InvalidOperationException("Zapremina boce mora biti veća od 0.");
            }

            if (dto.Cena.HasValue && dto.Cena.Value < 0)
            {
                throw new InvalidOperationException("Cena ne može biti negativna.");
            }

            var kreiraneBoceIds = new List<int>();

            for (int i = 0; i < dto.BrojBoca; i++)
            {
                var boca = new Boca
                {
                    VinoIdvina = dto.VinoIdvina,
                    MagacinIdmag = dto.MagacinIdmag,
                    Zapremina = dto.Zapremina,
                    Cena = dto.Cena
                };

                _context.Bocas.Add(boca);
            }

            await _context.SaveChangesAsync();

            var kreiraneBoce = await _context.Bocas
                .Where(b => b.VinoIdvina == dto.VinoIdvina &&
                           b.MagacinIdmag == dto.MagacinIdmag)
                .OrderByDescending(b => b.Idboce)
                .Take(dto.BrojBoca)
                .Select(b => b.Idboce)
                .ToListAsync();

            return new BocePunjenjeResultDto
            {
                BrojKreiranihBoca = dto.BrojBoca,
                NazivVina = vino.Nazivvina,
                NazivMagacina = magacin.Nazivmag,
                KreiraneBoceIds = kreiraneBoce
            };
        }

        public async Task<List<BocaInventarDto>> GetAllBoceAsync()
        {
            var boce = await _context.Bocas
                .Include(b => b.VinoIdvinaNavigation)
                .Include(b => b.MagacinIdmagNavigation)
                .OrderByDescending(b => b.Idboce)
                .Select(b => new BocaInventarDto
                {
                    Idboce = b.Idboce,
                    Cena = b.Cena,
                    Zapremina = b.Zapremina,
                    NazivVina = b.VinoIdvinaNavigation != null ? b.VinoIdvinaNavigation.Nazivvina : "N/A",
                    TipVina = b.VinoIdvinaNavigation != null ? b.VinoIdvinaNavigation.Tipvina : "N/A",
                    NazivMagacina = b.MagacinIdmagNavigation != null ? b.MagacinIdmagNavigation.Nazivmag : "N/A"
                })
                .ToListAsync();

            return boce;
        }

        public async Task<List<BocaDto>> GetBoceByMagacinAsync(int magacinId)
        {
            var boce = await _context.Bocas
                .Include(b => b.VinoIdvinaNavigation)
                .Include(b => b.MagacinIdmagNavigation)
                .Where(b => b.MagacinIdmag == magacinId)
                .Select(b => new BocaDto
                {
                    Idboce = b.Idboce,
                    Cena = b.Cena,
                    Zapremina = b.Zapremina,
                    VinoIdvina = b.VinoIdvina,
                    NazivVina = b.VinoIdvinaNavigation != null ? b.VinoIdvinaNavigation.Nazivvina : "N/A",
                    TipVina = b.VinoIdvinaNavigation != null ? b.VinoIdvinaNavigation.Tipvina : "N/A",
                    MagacinIdmag = b.MagacinIdmag,
                    NazivMagacina = b.MagacinIdmagNavigation != null ? b.MagacinIdmagNavigation.Nazivmag : "N/A"
                })
                .ToListAsync();

            return boce;
        }

        public async Task<int> GetBrojBocaUMagacinuAsync(int magacinId)
        {
            return await _context.Bocas
                .Where(b => b.MagacinIdmag == magacinId)
                .CountAsync();
        }
    }
}


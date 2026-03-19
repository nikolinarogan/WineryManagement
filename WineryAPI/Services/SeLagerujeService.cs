using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;

namespace WineryAPI.Services
{
    public class SeLagerujeService : ISeLagerujeService
    {
        private readonly ApplicationDbContext _context;

        public SeLagerujeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DostupnoBureDto>> GetDostupnaBuradAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var buradi = await _context.Bures
                .Include(b => b.PodrumIdpodNavigation)
                .Include(b => b.SeLagerujes)
                .Select(b => new DostupnoBureDto
                {
                    Idbur = b.Idbur,
                    Oznakabur = b.Oznakabur,
                    Materijal = b.Materijal,
                    Zapremina = b.Zapremina,
                    NazivPodruma = b.PodrumIdpodNavigation != null ? b.PodrumIdpodNavigation.Nazivpod : "N/A",
                    JeZauzeto = b.SeLagerujes.Any(l => l.Datpraznjenja > today)
                })
                .OrderBy(b => b.NazivPodruma)
                .ThenBy(b => b.Oznakabur)
                .ToListAsync();

            return buradi;
        }

        public async Task<List<LagerovanoVinoDto>> GetLagerovanjaZaSirovoVinoAsync(int sirovovinoId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var lagerovanja = await _context.SeLagerujes
                .Include(l => l.SirovovinoIdsirvinaNavigation)
                .Include(l => l.BureIdburNavigation)
                    .ThenInclude(b => b.PodrumIdpodNavigation)
                .Where(l => l.SirovovinoIdsirvina == sirovovinoId)
                .Select(l => new LagerovanoVinoDto
                {
                    SirovovinoIdsirvina = l.SirovovinoIdsirvina,
                    NazivSirovogVina = l.SirovovinoIdsirvinaNavigation.Nazivsirvina,
                    BureIdbur = l.BureIdbur,
                    OznakaBureta = l.BureIdburNavigation.Oznakabur,
                    MaterijalBureta = l.BureIdburNavigation.Materijal,
                    ZapreminaBureta = l.BureIdburNavigation.Zapremina,
                    NazivPodruma = l.BureIdburNavigation.PodrumIdpodNavigation != null 
                        ? l.BureIdburNavigation.PodrumIdpodNavigation.Nazivpod 
                        : "N/A",
                    Datpunjenja = l.Datpunjenja,
                    Datpraznjenja = l.Datpraznjenja,
                    JeAktivno = l.Datpraznjenja > today,
                    BrojDana = (l.Datpraznjenja > today) 
                        ? (today.DayNumber - l.Datpunjenja.DayNumber)
                        : (l.Datpraznjenja.DayNumber - l.Datpunjenja.DayNumber)
                })
                .OrderByDescending(l => l.Datpunjenja)
                .ToListAsync();

            return lagerovanja;
        }

        public async Task<List<LagerovanoVinoDto>> GetSvaLagerovanjaAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var lagerovanja = await _context.SeLagerujes
                .Include(l => l.SirovovinoIdsirvinaNavigation)
                .Include(l => l.BureIdburNavigation)
                    .ThenInclude(b => b.PodrumIdpodNavigation)
                .Select(l => new LagerovanoVinoDto
                {
                    SirovovinoIdsirvina = l.SirovovinoIdsirvina,
                    NazivSirovogVina = l.SirovovinoIdsirvinaNavigation.Nazivsirvina,
                    BureIdbur = l.BureIdbur,
                    OznakaBureta = l.BureIdburNavigation.Oznakabur,
                    MaterijalBureta = l.BureIdburNavigation.Materijal,
                    ZapreminaBureta = l.BureIdburNavigation.Zapremina,
                    NazivPodruma = l.BureIdburNavigation.PodrumIdpodNavigation != null 
                        ? l.BureIdburNavigation.PodrumIdpodNavigation.Nazivpod 
                        : "N/A",
                    Datpunjenja = l.Datpunjenja,
                    Datpraznjenja = l.Datpraznjenja,
                    JeAktivno = l.Datpraznjenja > today,
                    BrojDana = (l.Datpraznjenja > today) 
                        ? (today.DayNumber - l.Datpunjenja.DayNumber)
                        : (l.Datpraznjenja.DayNumber - l.Datpunjenja.DayNumber)
                })
                .OrderByDescending(l => l.JeAktivno)
                .ThenByDescending(l => l.Datpunjenja)
                .ToListAsync();

            return lagerovanja;
        }

        public async Task<LagerovanoVinoDto> StartLagerovanjeAsync(CreateLagerovanjeDto dto)
        {
            var sirovoVino = await _context.Sirovovinos.FindAsync(dto.SirovovinoIdsirvina);
            if (sirovoVino == null)
            {
                throw new KeyNotFoundException($"Sirovo vino sa ID {dto.SirovovinoIdsirvina} nije pronađeno.");
            }

            var bure = await _context.Bures
                .Include(b => b.PodrumIdpodNavigation)
                .FirstOrDefaultAsync(b => b.Idbur == dto.BureIdbur);
            
            if (bure == null)
            {
                throw new KeyNotFoundException($"Bure sa ID {dto.BureIdbur} nije pronađeno.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var jeZauzeto = await _context.SeLagerujes
                .AnyAsync(l => l.BureIdbur == dto.BureIdbur && l.Datpraznjenja > today);

            if (jeZauzeto)
            {
                throw new InvalidOperationException($"Bure '{bure.Oznakabur}' je trenutno zauzeto.");
            }

            if (dto.Datpunjenja > today)
            {
                throw new InvalidOperationException("Datum punjenja ne može biti u budućnosti.");
            }

            var lagerovanje = new SeLageruje
            {
                SirovovinoIdsirvina = dto.SirovovinoIdsirvina,
                BureIdbur = dto.BureIdbur,
                Datpunjenja = dto.Datpunjenja,
                Datpraznjenja = DateOnly.MaxValue 
            };

            _context.SeLagerujes.Add(lagerovanje);
            await _context.SaveChangesAsync();

            return new LagerovanoVinoDto
            {
                SirovovinoIdsirvina = lagerovanje.SirovovinoIdsirvina,
                NazivSirovogVina = sirovoVino.Nazivsirvina,
                BureIdbur = lagerovanje.BureIdbur,
                OznakaBureta = bure.Oznakabur,
                MaterijalBureta = bure.Materijal,
                ZapreminaBureta = bure.Zapremina,
                NazivPodruma = bure.PodrumIdpodNavigation?.Nazivpod ?? "N/A",
                Datpunjenja = lagerovanje.Datpunjenja,
                Datpraznjenja = lagerovanje.Datpraznjenja,
                JeAktivno = true,
                BrojDana = today.DayNumber - lagerovanje.Datpunjenja.DayNumber
            };
        }

        public async Task UpdateLagerovanjeAsync(int sirovovinoId, int bureId, UpdateLagerovanjeDto dto)
        {
            var lagerovanje = await _context.SeLagerujes
                .FirstOrDefaultAsync(l => l.SirovovinoIdsirvina == sirovovinoId && l.BureIdbur == bureId);

            if (lagerovanje == null)
            {
                throw new KeyNotFoundException($"Lagerovanje za sirovo vino {sirovovinoId} u buretu {bureId} nije pronađeno.");
            }

            if (dto.Datpraznjenja <= lagerovanje.Datpunjenja)
            {
                throw new InvalidOperationException("Datum pražnjenja mora biti nakon datuma punjenja.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (dto.Datpraznjenja > today)
            {
                throw new InvalidOperationException("Datum pražnjenja ne može biti u budućnosti.");
            }

            lagerovanje.Datpraznjenja = dto.Datpraznjenja;
            await _context.SaveChangesAsync();
        }
    }
}


using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class DegustacijaService : IDegustacijaService
    {
        private readonly IDegustacijaRepository _degustacijaRepository;
        private readonly IVinoRepository _vinoRepository;
        private readonly ApplicationDbContext _context;

        public DegustacijaService(
            IDegustacijaRepository degustacijaRepository,
            IVinoRepository vinoRepository,
            ApplicationDbContext context)
        {
            _degustacijaRepository = degustacijaRepository;
            _vinoRepository = vinoRepository;
            _context = context;
        }

        public async Task<List<DegustacijaBasicDto>> GetAllDegustacijeAsync()
        {
            var degustacije = await _degustacijaRepository.GetAllDegustacijeAsync();

            return degustacije.Select(d => new DegustacijaBasicDto
            {
                Iddeg = d.Iddeg,
                Nazivdeg = d.Nazivdeg,
                Datdeg = d.Datdeg,
                Kapacitetdeg = d.Kapacitetdeg,
                BrojVina = d.VinoIdvinas.Count
            }).ToList();
        }

        public async Task<DegustacijaDto> GetDegustacijaByIdAsync(int id)
        {
            var degustacija = await _degustacijaRepository.GetDegustacijaByIdAsync(id);

            if (degustacija == null)
            {
                throw new KeyNotFoundException($"Degustacija sa ID {id} nije pronađena.");
            }

            return MapToDegustacijaDto(degustacija);
        }

        public async Task<DegustacijaDto> CreateDegustacijaAsync(CreateDegustacijaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nazivdeg))
            {
                throw new InvalidOperationException("Naziv degustacije je obavezan.");
            }

            if (dto.Datdeg < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new InvalidOperationException("Datum degustacije mora biti u budućnosti.");
            }

            if (dto.Kapacitetdeg <= 0)
            {
                throw new InvalidOperationException("Kapacitet degustacije mora biti veći od 0.");
            }

            var someljer = await _degustacijaRepository.GetSomleijerByIdAsync(dto.SomleijerIdzap);
            if (someljer == null)
            {
                throw new KeyNotFoundException($"Someljer sa ID {dto.SomleijerIdzap} nije pronađen.");
            }

            if (dto.VinaIds == null || !dto.VinaIds.Any())
            {
                throw new InvalidOperationException("Mora biti izabrano barem jedno vino za degustaciju.");
            }

            var vina = new List<Vino>();
            foreach (var vinoId in dto.VinaIds)
            {
                var vino = await _vinoRepository.GetVinoByIdAsync(vinoId);
                if (vino == null)
                {
                    throw new KeyNotFoundException($"Vino sa ID {vinoId} nije pronađeno.");
                }
                vina.Add(vino);
            }

            var degustacija = new Degustacija
            {
                Nazivdeg = dto.Nazivdeg,
                Datdeg = dto.Datdeg,
                Kapacitetdeg = dto.Kapacitetdeg,
                VinoIdvinas = vina,
                SomleijerIdzaps = new List<Somleijer> { someljer }
            };

            var createdDegustacija = await _degustacijaRepository.CreateDegustacijaAsync(degustacija);

            var result = await _degustacijaRepository.GetDegustacijaByIdAsync(createdDegustacija.Iddeg);
            return MapToDegustacijaDto(result!);
        }

        public async Task<DegustacijaDto> UpdateDegustacijaAsync(int id, UpdateDegustacijaDto dto)
        {
            var degustacija = await _degustacijaRepository.GetDegustacijaByIdAsync(id);
            if (degustacija == null)
            {
                throw new KeyNotFoundException($"Degustacija sa ID {id} nije pronađena.");
            }

            if (string.IsNullOrWhiteSpace(dto.Nazivdeg))
            {
                throw new InvalidOperationException("Naziv degustacije je obavezan.");
            }

            if (dto.Kapacitetdeg <= 0)
            {
                throw new InvalidOperationException("Kapacitet degustacije mora biti veći od 0.");
            }

            if (dto.VinaIds == null || !dto.VinaIds.Any())
            {
                throw new InvalidOperationException("Mora biti izabrano barem jedno vino za degustaciju.");
            }

            var vina = new List<Vino>();
            foreach (var vinoId in dto.VinaIds)
            {
                var vino = await _vinoRepository.GetVinoByIdAsync(vinoId);
                if (vino == null)
                {
                    throw new KeyNotFoundException($"Vino sa ID {vinoId} nije pronađeno.");
                }
                vina.Add(vino);
            }

            degustacija.Nazivdeg = dto.Nazivdeg;
            degustacija.Datdeg = dto.Datdeg;
            degustacija.Kapacitetdeg = dto.Kapacitetdeg;
            degustacija.VinoIdvinas = vina;

            await _degustacijaRepository.UpdateDegustacijaAsync(degustacija);

            var result = await _degustacijaRepository.GetDegustacijaByIdAsync(id);
            return MapToDegustacijaDto(result!);
        }

        public async Task DeleteDegustacijaAsync(int id)
        {
            var exists = await _degustacijaRepository.DegustacijaExistsAsync(id);
            if (!exists)
            {
                throw new KeyNotFoundException($"Degustacija sa ID {id} nije pronađena.");
            }

            await _degustacijaRepository.DeleteDegustacijaAsync(id);
        }

        private DegustacijaDto MapToDegustacijaDto(Degustacija degustacija)
        {
            return new DegustacijaDto
            {
                Iddeg = degustacija.Iddeg,
                Nazivdeg = degustacija.Nazivdeg,
                Datdeg = degustacija.Datdeg,
                Kapacitetdeg = degustacija.Kapacitetdeg,
                Vina = degustacija.VinoIdvinas.Select(v => new VinoBasicDto
                {
                    Idvina = v.Idvina,
                    Nazivvina = v.Nazivvina,
                    Tipvina = v.Tipvina
                }).ToList(),
                Somelijeri = degustacija.SomleijerIdzaps.Select(s => new SomleijerBasicDto
                {
                    Idzap = s.Idzap,
                    Ime = s.IdzapNavigation.Ime,
                    Prezime = s.IdzapNavigation.Prez,
                    Specijalnost = s.Specijalnost
                }).ToList()
            };
        }
    }
}


using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class VinoService : IVinoService
    {
        private readonly IVinoRepository _vinoRepository;

        public VinoService(IVinoRepository vinoRepository)
        {
            _vinoRepository = vinoRepository;
        }

        public async Task<List<VinoDto>> GetAllVinaAsync()
        {
            var vina = await _vinoRepository.GetAllVinaAsync();

            return vina.Select(v => new VinoDto
            {
                Idvina = v.Idvina,
                Nazivvina = v.Nazivvina,
                Procalk = v.Procalk,
                Tipvina = v.Tipvina,
                BrojSirovihVina = v.SirovovinoIdsirvinas.Count,
                SirovaVina = v.SirovovinoIdsirvinas.Select(sv => new SirovoVinoUVinuDto
                {
                    Idsirvina = sv.Idsirvina,
                    Nazivsirvina = sv.Nazivsirvina,
                    Kolicinasirvina = sv.Kolicinasirvina,
                    Kvalitet = sv.Kvalitet,
                    Godproizvodnje = sv.Godproizvodnje
                }).ToList()
            }).ToList();
        }

        public async Task<VinoDetailDto?> GetVinoByIdAsync(int id)
        {
            var vino = await _vinoRepository.GetVinoWithSirovimVinimaAsync(id);

            if (vino == null)
                return null;

            return new VinoDetailDto
            {
                Idvina = vino.Idvina,
                Nazivvina = vino.Nazivvina,
                Procalk = vino.Procalk,
                Tipvina = vino.Tipvina,
                SirovaVina = vino.SirovovinoIdsirvinas.Select(sv => new SirovoVinoUVinuDto
                {
                    Idsirvina = sv.Idsirvina,
                    Nazivsirvina = sv.Nazivsirvina,
                    Kolicinasirvina = sv.Kolicinasirvina,
                    Kvalitet = sv.Kvalitet,
                    Godproizvodnje = sv.Godproizvodnje
                }).ToList()
            };
        }

        public async Task<VinoDto> CreateVinoAsync(CreateVinoDto dto)
        {
            if (dto.SirovaVinaIds == null || !dto.SirovaVinaIds.Any())
            {
                throw new InvalidOperationException("Finalno vino mora biti kreirano od bar jednog sirovog vina.");
            }

            var sirovaVina = await _vinoRepository.GetSirovaVinaByIdsAsync(dto.SirovaVinaIds);

            if (sirovaVina.Count != dto.SirovaVinaIds.Count)
            {
                throw new KeyNotFoundException("Jedno ili više sirovih vina nije pronađeno.");
            }

            var postojiNaziv = await _vinoRepository.VinoExistsByNameAsync(dto.Nazivvina);

            if (postojiNaziv)
            {
                throw new InvalidOperationException($"Vino sa nazivom '{dto.Nazivvina}' već postoji.");
            }

            var vino = new Vino
            {
                Nazivvina = dto.Nazivvina,
                Procalk = dto.Procalk,
                Tipvina = dto.Tipvina
            };

            foreach (var sirovoVino in sirovaVina)
            {
                vino.SirovovinoIdsirvinas.Add(sirovoVino);
            }

            await _vinoRepository.AddVinoAsync(vino);

            return new VinoDto
            {
                Idvina = vino.Idvina,
                Nazivvina = vino.Nazivvina,
                Procalk = vino.Procalk,
                Tipvina = vino.Tipvina,
                BrojSirovihVina = sirovaVina.Count,
                SirovaVina = sirovaVina.Select(sv => new SirovoVinoUVinuDto
                {
                    Idsirvina = sv.Idsirvina,
                    Nazivsirvina = sv.Nazivsirvina,
                    Kolicinasirvina = sv.Kolicinasirvina,
                    Kvalitet = sv.Kvalitet,
                    Godproizvodnje = sv.Godproizvodnje
                }).ToList()
            };
        }

        public async Task UpdateVinoAsync(int id, UpdateVinoDto dto)
        {
            var vino = await _vinoRepository.GetVinoByIdAsync(id);

            if (vino == null)
            {
                throw new KeyNotFoundException($"Vino sa ID {id} nije pronađeno.");
            }

            var postojiNaziv = await _vinoRepository.VinoExistsByNameAsync(dto.Nazivvina, id);

            if (postojiNaziv)
            {
                throw new InvalidOperationException($"Vino sa nazivom '{dto.Nazivvina}' već postoji.");
            }

            vino.Nazivvina = dto.Nazivvina;
            vino.Procalk = dto.Procalk;
            vino.Tipvina = dto.Tipvina;

            await _vinoRepository.UpdateVinoAsync(vino);
        }

        public async Task DeleteVinoAsync(int id)
        {
            var vino = await _vinoRepository.GetVinoForDeletionAsync(id);

            if (vino == null)
            {
                throw new KeyNotFoundException($"Vino sa ID {id} nije pronađeno.");
            }

            if (vino.Bocas.Any())
            {
                throw new InvalidOperationException($"Vino '{vino.Nazivvina}' ne može biti obrisano jer postoje boce ovog vina.");
            }

            if (vino.Ucestvujes.Any())
            {
                throw new InvalidOperationException($"Vino '{vino.Nazivvina}' ne može biti obrisano jer učestvuje u nekim događajima.");
            }

            if (vino.DegustacijaIddegs.Any())
            {
                throw new InvalidOperationException($"Vino '{vino.Nazivvina}' ne može biti obrisano jer je deo degustacija.");
            }

            await _vinoRepository.DeleteVinoAsync(vino);
        }
    }
}


using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class SirovinazatretmanService : ISirovinazatretmanService
    {
        private readonly ISirovinazatretmanRepository _sirovinazatretmanRepository;

        public SirovinazatretmanService(ISirovinazatretmanRepository sirovinazatretmanRepository)
        {
            _sirovinazatretmanRepository = sirovinazatretmanRepository;
        }

        public async Task<List<SirovinazatretmanDto>> GetAllSirovineAsync()
        {
            var sirovine = await _sirovinazatretmanRepository.GetAllSirovineAsync();

            return sirovine.Select(s => new SirovinazatretmanDto
            {
                Idsir = s.Idsir,
                Naziv = s.Naziv,
                BrojKoriscenjaUTretmanima = s.SeDodajes.Count
            }).ToList();
        }

        public async Task<SirovinazatretmanDto?> GetSirovinaByIdAsync(int id)
        {
            var sirovina = await _sirovinazatretmanRepository.GetSirovinaWithUsageAsync(id);

            if (sirovina == null)
                return null;

            return new SirovinazatretmanDto
            {
                Idsir = sirovina.Idsir,
                Naziv = sirovina.Naziv,
                BrojKoriscenjaUTretmanima = sirovina.SeDodajes.Count
            };
        }

        public async Task<SirovinazatretmanDto> CreateSirovinaAsync(CreateSirovinazatretmanDto dto)
        {
            var postojiNaziv = await _sirovinazatretmanRepository.SirovinaExistsByNameAsync(dto.Naziv);

            if (postojiNaziv)
                throw new InvalidOperationException($"Sirovina sa nazivom '{dto.Naziv}' već postoji u katalogu.");

            var sirovina = new Sirovinazatretman
            {
                Naziv = dto.Naziv
            };

            await _sirovinazatretmanRepository.AddSirovinaAsync(sirovina);

            return new SirovinazatretmanDto
            {
                Idsir = sirovina.Idsir,
                Naziv = sirovina.Naziv,
                BrojKoriscenjaUTretmanima = 0
            };
        }

        public async Task UpdateSirovinaAsync(int id, UpdateSirovinazatretmanDto dto)
        {
            var sirovina = await _sirovinazatretmanRepository.GetSirovinaByIdAsync(id);

            if (sirovina == null)
                throw new KeyNotFoundException($"Sirovina sa ID {id} nije pronađena.");

            var postojiNaziv = await _sirovinazatretmanRepository.SirovinaExistsByNameAsync(dto.Naziv, id);

            if (postojiNaziv)
                throw new InvalidOperationException($"Sirovina sa nazivom '{dto.Naziv}' već postoji u katalogu.");

            sirovina.Naziv = dto.Naziv;

            await _sirovinazatretmanRepository.UpdateSirovinaAsync(sirovina);
        }

        public async Task DeleteSirovinaAsync(int id)
        {
            var sirovina = await _sirovinazatretmanRepository.GetSirovinaWithUsageAsync(id);

            if (sirovina == null)
                throw new KeyNotFoundException($"Sirovina sa ID {id} nije pronađena.");

            if (sirovina.SeDodajes.Any())
            {
                throw new InvalidOperationException(
                    $"Sirovina '{sirovina.Naziv}' ne može biti obrisana jer je korišćena u {sirovina.SeDodajes.Count} tretmana.");
            }

            await _sirovinazatretmanRepository.DeleteSirovinaAsync(sirovina);
        }
    }
}


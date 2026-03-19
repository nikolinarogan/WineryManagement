using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class SortagrozdjaService : ISortagrozdjaService
    {
        private readonly ISortagrozdjaRepository _sortagrozdjaRepository;

        public SortagrozdjaService(ISortagrozdjaRepository sortagrozdjaRepository)
        {
            _sortagrozdjaRepository = sortagrozdjaRepository;
        }

        public async Task<List<SortagrozdjaDto>> GetAllSorteAsync()
        {
            var sorte = await _sortagrozdjaRepository.GetAllSorteAsync();

            return sorte.Select(s => new SortagrozdjaDto
            {
                Idsrt = s.Idsrt,
                Nazivsorte = s.Nazivsorte,
                Bojasorte = s.Bojasorte,
                Porijeklosorte = s.Porijeklosorte,
                Periodsazr = s.Periodsazr,
                Kiselost = s.Kiselost,
                BrojParcela = s.Parcelas.Count
            }).ToList();
        }

        public async Task<SortagrozdjaDetailDto?> GetSortaByIdAsync(int id)
        {
            var sorta = await _sortagrozdjaRepository.GetSortaWithParcelaByIdAsync(id);

            if (sorta == null)
                return null;

            return new SortagrozdjaDetailDto
            {
                Idsrt = sorta.Idsrt,
                Nazivsorte = sorta.Nazivsorte,
                Bojasorte = sorta.Bojasorte,
                Porijeklosorte = sorta.Porijeklosorte,
                Periodsazr = sorta.Periodsazr,
                Kiselost = sorta.Kiselost,
                Parcele = sorta.Parcelas.Select(p => new ParcelaInfoDto
                {
                    Idp = p.Idp,
                    Nazivparcele = p.Nazivparcele,
                    Povrsina = p.Povrsina,
                    Brojcokota = p.Brojcokota,
                    VinogradNaziv = p.VinogradIdvNavigation.Naziv,
                    VinogradIdv = p.VinogradIdv
                }).ToList()
            };
        }

        public async Task<SortagrozdjaDto> CreateSortaAsync(CreateSortagrozdjaDto dto)
        {
            var sorta = new Sortagrozdja
            {
                Nazivsorte = dto.Nazivsorte,
                Bojasorte = dto.Bojasorte,
                Porijeklosorte = dto.Porijeklosorte,
                Periodsazr = dto.Periodsazr,
                Kiselost = dto.Kiselost
            };

            sorta = await _sortagrozdjaRepository.AddSortaAsync(sorta);

            return new SortagrozdjaDto
            {
                Idsrt = sorta.Idsrt,
                Nazivsorte = sorta.Nazivsorte,
                Bojasorte = sorta.Bojasorte,
                Porijeklosorte = sorta.Porijeklosorte,
                Periodsazr = sorta.Periodsazr,
                Kiselost = sorta.Kiselost,
                BrojParcela = 0
            };
        }

        public async Task<bool> UpdateSortaAsync(int id, UpdateSortagrozdjaDto dto)
        {
            var sorta = await _sortagrozdjaRepository.GetSortaByIdAsync(id);
            if (sorta == null)
                return false;

            sorta.Nazivsorte = dto.Nazivsorte;
            sorta.Bojasorte = dto.Bojasorte;
            sorta.Porijeklosorte = dto.Porijeklosorte;
            sorta.Periodsazr = dto.Periodsazr;
            sorta.Kiselost = dto.Kiselost;

            await _sortagrozdjaRepository.UpdateSortaAsync(sorta);
            return true;
        }

        public async Task<bool> DeleteSortaAsync(int id)
        {
            var sorta = await _sortagrozdjaRepository.GetSortaByIdAsync(id);
            if (sorta == null)
                return false;

            await _sortagrozdjaRepository.DeleteSortaAsync(sorta);
            return true;
        }
    }
}


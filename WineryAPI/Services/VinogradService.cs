using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class VinogradService : IVinogradService
    {
        private readonly IVinogradRepository _vinogradRepository;

        public VinogradService(IVinogradRepository vinogradRepository)
        {
            _vinogradRepository = vinogradRepository;
        }

        public async Task<List<VinogradDto>> GetAllVinogradiAsync()
        {
            var vinogradi = await _vinogradRepository.GetAllVinogradiAsync();

            return vinogradi.Select(v => new VinogradDto
            {
                Idv = v.Idv,
                Naziv = v.Naziv,
                Datosn = v.Datosn,
                Povrsina = v.Povrsina,
                Nadmorskavis = v.Nadmorskavis,
                Tipzemljista = v.Tipzemljista,
                Navodnjavanje = v.Navodnjavanje,
                BrojParcela = v.Parcelas.Count
            }).ToList();
        }

        public async Task<List<VinogradDetailDto>> GetAllVinogradiWithParcelaAsync()
        {
            var vinogradi = await _vinogradRepository.GetAllVinogradiWithParcelaAsync();

            return vinogradi.Select(v => new VinogradDetailDto
            {
                Idv = v.Idv,
                Naziv = v.Naziv,
                Datosn = v.Datosn,
                Povrsina = v.Povrsina,
                Nadmorskavis = v.Nadmorskavis,
                Tipzemljista = v.Tipzemljista,
                Navodnjavanje = v.Navodnjavanje,
                Parcele = v.Parcelas.Select(p => new ParcelaDto
                {
                    Idp = p.Idp,
                    Brojcokota = p.Brojcokota,
                    Povrsina = p.Povrsina,
                    Nazivparcele = p.Nazivparcele,
                    VinogradIdv = p.VinogradIdv,
                    SortagrozdjaIdsrt = p.SortagrozdjaIdsrt,
                    SortaNaziv = p.SortagrozdjaIdsrtNavigation?.Nazivsorte
                }).ToList()
            }).ToList();
        }

        public async Task<VinogradDetailDto?> GetVinogradByIdAsync(int id)
        {
            var vinogradDetail = await _vinogradRepository.GetVinogradWithParcelaByIdAsync(id);

            if (vinogradDetail == null)
                return null;

            return new VinogradDetailDto
            {
                Idv = vinogradDetail.Idv,
                Naziv = vinogradDetail.Naziv,
                Datosn = vinogradDetail.Datosn,
                Povrsina = vinogradDetail.Povrsina,
                Nadmorskavis = vinogradDetail.Nadmorskavis,
                Tipzemljista = vinogradDetail.Tipzemljista,
                Navodnjavanje = vinogradDetail.Navodnjavanje,
                Parcele = vinogradDetail.Parcelas.Select(p => new ParcelaDto
                {
                    Idp = p.Idp,
                    Brojcokota = p.Brojcokota,
                    Povrsina = p.Povrsina,
                    Nazivparcele = p.Nazivparcele,
                    VinogradIdv = p.VinogradIdv,
                    SortagrozdjaIdsrt = p.SortagrozdjaIdsrt,
                    SortaNaziv = p.SortagrozdjaIdsrtNavigation?.Nazivsorte
                }).ToList()
            };
        }

        public async Task<VinogradDto> CreateVinogradAsync(CreateVinogradDto dto)
        {
            var vinograd = new Vinograd
            {
                Naziv = dto.Naziv,
                Datosn = dto.Datosn,
                Povrsina = dto.Povrsina,
                Nadmorskavis = dto.Nadmorskavis,
                Tipzemljista = dto.Tipzemljista,
                Navodnjavanje = dto.Navodnjavanje
            };

            vinograd = await _vinogradRepository.AddVinogradAsync(vinograd);

            if (dto.Parcele != null && dto.Parcele.Any())
            {
                foreach (var parcelaDto in dto.Parcele)
                {
                    var parcela = new Parcela
                    {
                        Brojcokota = parcelaDto.Brojcokota,
                        Povrsina = parcelaDto.Povrsina,
                        Nazivparcele = parcelaDto.Nazivparcele,
                        VinogradIdv = vinograd.Idv,
                        SortagrozdjaIdsrt = parcelaDto.SortagrozdjaIdsrt
                    };
                    await _vinogradRepository.AddParcelaAsync(parcela);
                }
            }

            return new VinogradDto
            {
                Idv = vinograd.Idv,
                Naziv = vinograd.Naziv,
                Datosn = vinograd.Datosn,
                Povrsina = vinograd.Povrsina,
                Nadmorskavis = vinograd.Nadmorskavis,
                Tipzemljista = vinograd.Tipzemljista,
                Navodnjavanje = vinograd.Navodnjavanje,
                BrojParcela = dto.Parcele?.Count ?? 0
            };
        }

        public async Task<bool> UpdateVinogradAsync(int id, UpdateVinogradDto dto)
        {
            var vinograd = await _vinogradRepository.GetVinogradByIdAsync(id);
            if (vinograd == null)
                return false;

            vinograd.Naziv = dto.Naziv;
            vinograd.Datosn = dto.Datosn;
            vinograd.Povrsina = dto.Povrsina;
            vinograd.Nadmorskavis = dto.Nadmorskavis;
            vinograd.Tipzemljista = dto.Tipzemljista;
            vinograd.Navodnjavanje = dto.Navodnjavanje;

            await _vinogradRepository.UpdateVinogradAsync(vinograd);
            return true;
        }

        public async Task<bool> DeleteVinogradAsync(int id)
        {
            var vinograd = await _vinogradRepository.GetVinogradByIdAsync(id);
            if (vinograd == null)
                return false;

            await _vinogradRepository.DeleteVinogradAsync(vinograd);
            return true;
        }

        public async Task<ParcelaDto> AddParcelaToVinogradAsync(int vinogradId, CreateParcelaDto dto)
        {
            var vinograd = await _vinogradRepository.GetVinogradByIdAsync(vinogradId);
            if (vinograd == null)
                throw new InvalidOperationException("Vinograd nije pronađen");

            var parcela = new Parcela
            {
                Brojcokota = dto.Brojcokota,
                Povrsina = dto.Povrsina,
                Nazivparcele = dto.Nazivparcele,
                VinogradIdv = vinogradId,
                SortagrozdjaIdsrt = dto.SortagrozdjaIdsrt
            };

            parcela = await _vinogradRepository.AddParcelaAsync(parcela);

            return new ParcelaDto
            {
                Idp = parcela.Idp,
                Brojcokota = parcela.Brojcokota,
                Povrsina = parcela.Povrsina,
                Nazivparcele = parcela.Nazivparcele,
                VinogradIdv = parcela.VinogradIdv,
                SortagrozdjaIdsrt = parcela.SortagrozdjaIdsrt
            };
        }

        public async Task<bool> UpdateParcelaAsync(int parcelaId, UpdateParcelaDto dto)
        {
            var parcela = await _vinogradRepository.GetParcelaByIdAsync(parcelaId);
            if (parcela == null)
                return false;

            parcela.Brojcokota = dto.Brojcokota;
            parcela.Povrsina = dto.Povrsina;
            parcela.Nazivparcele = dto.Nazivparcele;
            parcela.SortagrozdjaIdsrt = dto.SortagrozdjaIdsrt;

            await _vinogradRepository.UpdateParcelaAsync(parcela);
            return true;
        }

        public async Task<bool> DeleteParcelaAsync(int parcelaId)
        {
            var parcela = await _vinogradRepository.GetParcelaByIdAsync(parcelaId);
            if (parcela == null)
                return false;

            await _vinogradRepository.DeleteParcelaAsync(parcela);
            return true;
        }
    }
}

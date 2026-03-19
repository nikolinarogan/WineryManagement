using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class RadoviService : IRadoviService
    {
        private readonly IRadoviRepository _radoviRepository;
        private readonly IBerbaRepository _berbaRepository;

        public RadoviService(IRadoviRepository radoviRepository, IBerbaRepository berbaRepository)
        {
            _radoviRepository = radoviRepository;
            _berbaRepository = berbaRepository;
        }

        public async Task<List<RadoviDto>> GetAllRadoviAsync()
        {
            var radovi = await _radoviRepository.GetAllRadoviAsync();

            return radovi.Select(r => new RadoviDto
            {
                Idrad = r.Idrad,
                Pocrad = r.Pocrad,
                Zavrrad = r.Zavrrad,
                Oprema = r.Oprema,
                BrojParcela = r.ParcelaIdps.Count,
                BrojRadnika = r.RadnikIdzaps.Count
            }).ToList();
        }

        public async Task<RadoviDetailDto?> GetRadoviByIdAsync(int id)
        {
            var rad = await _radoviRepository.GetRadoviWithDetailsAsync(id);

            if (rad == null)
                return null;

            return new RadoviDetailDto
            {
                Idrad = rad.Idrad,
                Pocrad = rad.Pocrad,
                Zavrrad = rad.Zavrrad,
                Oprema = rad.Oprema,
                Parcele = rad.ParcelaIdps.Select(p => new ParcelaNaRaduDto
                {
                    Idp = p.Idp,
                    Nazivparcele = p.Nazivparcele,
                    VinogradNaziv = p.VinogradIdvNavigation.Naziv,
                    Povrsina = p.Povrsina
                }).ToList(),
                Radnici = rad.RadnikIdzaps.Select(r => new RadnikNaRaduDto
                {
                    RadnikIdzap = r.Idzap,
                    Ime = r.IdzapNavigation.Ime,
                    Prezime = r.IdzapNavigation.Prez,
                    Email = r.IdzapNavigation.Email
                }).ToList()
            };
        }

        public async Task<RadoviDto> CreateRadoviAsync(CreateRadoviDto dto)
        {
            var rad = new Radovi
            {
                Pocrad = dto.Pocrad,
                Zavrrad = dto.Zavrrad,
                Oprema = dto.Oprema
            };

            await _radoviRepository.AddRadoviAsync(rad);

            if (dto.ParcelaIds != null && dto.ParcelaIds.Any())
            {
                var radWithParcele = await _radoviRepository.GetRadoviWithParcelaAsync(rad.Idrad);
                
                foreach (var parcelaId in dto.ParcelaIds)
                {
                    var parcela = await _radoviRepository.GetParcelaByIdAsync(parcelaId);
                    if (parcela != null)
                    {
                        radWithParcele!.ParcelaIdps.Add(parcela);
                    }
                }

                await _radoviRepository.SaveChangesAsync();
            }

            return new RadoviDto
            {
                Idrad = rad.Idrad,
                Pocrad = rad.Pocrad,
                Zavrrad = rad.Zavrrad,
                Oprema = rad.Oprema,
                BrojParcela = dto.ParcelaIds?.Count ?? 0,
                BrojRadnika = 0
            };
        }

        public async Task<bool> UpdateRadoviAsync(int id, UpdateRadoviDto dto)
        {
            var rad = await _radoviRepository.GetRadoviByIdAsync(id);
            if (rad == null)
                return false;

            rad.Pocrad = dto.Pocrad;
            rad.Zavrrad = dto.Zavrrad;
            rad.Oprema = dto.Oprema;

            await _radoviRepository.UpdateRadoviAsync(rad);
            return true;
        }

        public async Task<bool> DeleteRadoviAsync(int id)
        {
            var rad = await _radoviRepository.GetRadoviByIdAsync(id);
            if (rad == null)
                return false;

            await _radoviRepository.DeleteRadoviAsync(rad);
            return true;
        }

        public async Task<bool> AddParcelaToRadAsync(int radId, int parcelaId)
        {
            var rad = await _radoviRepository.GetRadoviWithParcelaAsync(radId);

            if (rad == null)
                return false;

            var parcela = await _radoviRepository.GetParcelaByIdAsync(parcelaId);
            if (parcela == null)
                return false;

            if (rad.ParcelaIdps.Any(p => p.Idp == parcelaId))
                throw new InvalidOperationException("Parcela je već dodana na ovaj rad");

            rad.ParcelaIdps.Add(parcela);
            await _radoviRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveParcelaFromRadAsync(int radId, int parcelaId)
        {
            var rad = await _radoviRepository.GetRadoviWithParcelaAsync(radId);

            if (rad == null)
                return false;

            var parcela = rad.ParcelaIdps.FirstOrDefault(p => p.Idp == parcelaId);
            if (parcela == null)
                return false;

            rad.ParcelaIdps.Remove(parcela);
            await _radoviRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddRadnikToRadAsync(int radId, AddRadnikToRadDto dto)
        {
            var rad = await _radoviRepository.GetRadoviWithRadniciAsync(radId);

            if (rad == null)
                return false;

            var radnik = await _radoviRepository.GetRadnikByIdAsync(dto.RadnikIdzap);
            if (radnik == null)
                return false;

            if (rad.RadnikIdzaps.Any(r => r.Idzap == dto.RadnikIdzap))
                throw new InvalidOperationException("Radnik je već dodijeljen ovom radu");

            var pocetakNovog = rad.Pocrad;
            var zavrsetakNovog = rad.Zavrrad;

            var radnikRadovi = await _radoviRepository.GetRadoviForRadnikAsync(dto.RadnikIdzap);
            
            var preklapajuciRad = radnikRadovi
                .Where(r => r.Pocrad <= zavrsetakNovog && r.Zavrrad >= pocetakNovog && r.Idrad != radId)
                .FirstOrDefault();

            if (preklapajuciRad != null)
            {
                var parcelaNames = string.Join(", ", preklapajuciRad.ParcelaIdps.Take(2).Select(p => p.Nazivparcele));
                throw new InvalidOperationException(
                    $"Radnik već ima zakazan rad u periodu od {preklapajuciRad.Pocrad:dd.MM.yyyy} " +
                    $"do {preklapajuciRad.Zavrrad:dd.MM.yyyy} " +
                    $"na parcelama: {parcelaNames}"
                );
            }

            var preklapajuciRasporedi = await _berbaRepository.GetRasporedbranjaForRadnikInDateRangeAsync(
                dto.RadnikIdzap,
                pocetakNovog,
                zavrsetakNovog
            );

            if (preklapajuciRasporedi.Any())
            {
                var raspored = preklapajuciRasporedi.First();
                var berba = raspored.Seodrzava?.BerbaIdberNavigation;
                var parcela = raspored.Seodrzava?.ParcelaIdpNavigation;
                throw new InvalidOperationException(
                    $"Radnik već ima zakazan raspored branja u periodu od {raspored.Pocbranja:dd.MM.yyyy} " +
                    $"do {raspored.Zavrsetakbranja:dd.MM.yyyy} " +
                    $"za berbu '{berba?.Nazber}' na parceli '{parcela?.Nazivparcele}'"
                );
            }

            rad.RadnikIdzaps.Add(radnik);
            await _radoviRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRadnikFromRadAsync(int radId, int radnikId)
        {
            var rad = await _radoviRepository.GetRadoviWithRadniciAsync(radId);

            if (rad == null)
                return false;

            var radnik = rad.RadnikIdzaps.FirstOrDefault(r => r.Idzap == radnikId);
            if (radnik == null)
                return false;

            rad.RadnikIdzaps.Remove(radnik);
            await _radoviRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<RadnikRadDto>> GetRadoviForRadnikAsync(int radnikId)
        {
            var radnik = await _radoviRepository.GetRadnikWithRadoviAsync(radnikId);

            if (radnik == null)
                return new List<RadnikRadDto>();

            return radnik.RadoviIdrads.Select(rad => new RadnikRadDto
            {
                Idrad = rad.Idrad,
                Pocrad = rad.Pocrad,
                Zavrrad = rad.Zavrrad,
                Oprema = rad.Oprema,
                Parcele = rad.ParcelaIdps.Select(p => new ParcelaNaRaduDto
                {
                    Idp = p.Idp,
                    Nazivparcele = p.Nazivparcele,
                    VinogradNaziv = p.VinogradIdvNavigation.Naziv,
                    Povrsina = p.Povrsina
                }).ToList()
            }).ToList();
        }
    }
}


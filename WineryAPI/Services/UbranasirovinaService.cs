using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class UbranasirovinaService : IUbranasirovinaService
    {
        private readonly IUbranasirovinaRepository _ubranasirovinaRepository;

        public UbranasirovinaService(IUbranasirovinaRepository ubranasirovinaRepository)
        {
            _ubranasirovinaRepository = ubranasirovinaRepository;
        }

        public async Task<List<UbranasirovinaDto>> GetAllPrijemiAsync()
        {
            var prijemi = await _ubranasirovinaRepository.GetAllUbranasirovinaAsync();

            return prijemi.Select(u => new UbranasirovinaDto
            {
                Idubrsir = u.Idubrsir,
                Kolicina = u.Kolicina,
                Datprijema = u.Datprijema,
                RasporedbranjaIdras = u.RasporedbranjaIdras,
                BerbaNaziv = u.RasporedbranjaIdrasNavigation.Seodrzava.BerbaIdberNavigation.Nazber,
                ParcelaNaziv = u.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                VinogradNaziv = u.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.VinogradIdvNavigation.Naziv,
                SortaNaziv = u.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.SortagrozdjaIdsrtNavigation != null
                    ? u.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.SortagrozdjaIdsrtNavigation.Nazivsorte
                    : null
            }).ToList();
        }

        public async Task<UbranasirovinaDto?> GetPrijemByIdAsync(int id)
        {
            var prijem = await _ubranasirovinaRepository.GetUbranasirovinaWithDetailsAsync(id);

            if (prijem == null)
                return null;

            return new UbranasirovinaDto
            {
                Idubrsir = prijem.Idubrsir,
                Kolicina = prijem.Kolicina,
                Datprijema = prijem.Datprijema,
                RasporedbranjaIdras = prijem.RasporedbranjaIdras,
                BerbaNaziv = prijem.RasporedbranjaIdrasNavigation.Seodrzava.BerbaIdberNavigation.Nazber,
                ParcelaNaziv = prijem.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                VinogradNaziv = prijem.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.VinogradIdvNavigation.Naziv,
                SortaNaziv = prijem.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.SortagrozdjaIdsrtNavigation?.Nazivsorte
            };
        }

        public async Task<List<RasporedZaPrijemDto>> GetRasporedsReadyForPrijemAsync()
        {
            var rasporedi = await _ubranasirovinaRepository.GetRasporedbranjaForPrijemAsync();

            return rasporedi.Select(r =>
            {
                var ukupnoUbrano = r.JeAngazovans.Sum(ja => ja.Kolicinaubrgr);
                var brojRadnika = r.JeAngazovans.Count;
                var brojSaUnosom = r.JeAngazovans.Count(ja => ja.Kolicinaubrgr > 0); 

                return new RasporedZaPrijemDto
                {
                    RasporedId = r.Idras,
                    BerbaNaziv = r.Seodrzava.BerbaIdberNavigation.Nazber,
                    Sezona = r.Seodrzava.BerbaIdberNavigation.Sezona,
                    ParcelaNaziv = r.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                    VinogradNaziv = r.Seodrzava.ParcelaIdpNavigation.VinogradIdvNavigation.Naziv,
                    SortaNaziv = r.Seodrzava.ParcelaIdpNavigation.SortagrozdjaIdsrtNavigation?.Nazivsorte,
                    PocetakBranja = r.Pocbranja,
                    ZavrsetakBranja = r.Zavrsetakbranja,
                    OcekivaniPrinos = r.Ocekivaniprinos,
                    UkupnoUbrano = ukupnoUbrano,
                    ProcenatRealizacije = r.Ocekivaniprinos > 0
                        ? (ukupnoUbrano / r.Ocekivaniprinos) * 100
                        : 0,
                    UkupnoRadnika = brojRadnika,
                    RadnikaSaUnosom = brojSaUnosom
                };
            })
            .OrderBy(r => r.ZavrsetakBranja)
            .ToList();
        }

        public async Task<PrijemDetailsDto?> GetPrijemDetailsAsync(int rasporedId)
        {
            var raspored = await _ubranasirovinaRepository.GetRasporedbranjaWithFullDetailsAsync(rasporedId);

            if (raspored == null)
                return null;

            var kolicine = raspored.JeAngazovans.Select(ja => ja.Kolicinaubrgr).ToList();
            var ukupnoUbrano = kolicine.Sum();
            var prosek = kolicine.Any() ? kolicine.Average() : 0;
            var stdDev = CalculateStdDev(kolicine);

            // Detekcija anomalija
            var radniciSaKolicinama = raspored.JeAngazovans.Select(ja =>
            {
                var odstupanje = prosek > 0 ? ((ja.Kolicinaubrgr - prosek) / prosek) * 100 : 0;
                var isOutlier = Math.Abs(ja.Kolicinaubrgr - prosek) > 0.3m * prosek;

                return new RadnikKolicinaDto
                {
                    RadnikId = ja.RadnikIdzap,
                    Ime = ja.RadnikIdzapNavigation.IdzapNavigation.Ime,
                    Prezime = ja.RadnikIdzapNavigation.IdzapNavigation.Prez,
                    Kolicina = ja.Kolicinaubrgr,
                    Datum = ja.Datumbranja,
                    OdstupostotakOdProseka = odstupanje,
                    IsOutlier = isOutlier
                };
            }).ToList();

            var anomalije = radniciSaKolicinama
                .Where(r => r.IsOutlier)
                .Select(r => new RadnikAnomalijaDto
                {
                    RadnikId = r.RadnikId,
                    Ime = r.Ime,
                    Prezime = r.Prezime,
                    Kolicina = r.Kolicina,
                    Prosek = prosek,
                    OdstupostotakPostotak = r.OdstupostotakOdProseka,
                    Tip = r.OdstupostotakOdProseka > 0 ? "IZNAD_PROSEKA" : "ISPOD_PROSEKA",
                    Poruka = $"Odstupanje: {Math.Abs(r.OdstupostotakOdProseka):F1}% od proseka ({prosek:F2} kg)"
                })
                .ToList();

            return new PrijemDetailsDto
            {
                RasporedId = raspored.Idras,
                BerbaNaziv = raspored.Seodrzava.BerbaIdberNavigation.Nazber,
                ParcelaNaziv = raspored.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                VinogradNaziv = raspored.Seodrzava.ParcelaIdpNavigation.VinogradIdvNavigation.Naziv,
                SortaNaziv = raspored.Seodrzava.ParcelaIdpNavigation.SortagrozdjaIdsrtNavigation?.Nazivsorte,
                PocetakBranja = raspored.Pocbranja,
                ZavrsetakBranja = raspored.Zavrsetakbranja,
                OcekivaniPrinos = raspored.Ocekivaniprinos,
                UkupnoUbrano = ukupnoUbrano,
                ProcenatRealizacije = raspored.Ocekivaniprinos > 0
                    ? (ukupnoUbrano / raspored.Ocekivaniprinos) * 100
                    : 0,
                ProsekPoRadniku = prosek,
                StandardnaDevijacija = stdDev,
                Radnici = radniciSaKolicinama,
                BrojAnomalija = anomalije.Count,
                Anomalije = anomalije,
                PreporucenaKolicina = ukupnoUbrano * 0.97m,
                MinValidnaKolicina = ukupnoUbrano * 0.85m,
                MaxValidnaKolicina = ukupnoUbrano
            };
        }

        public async Task<PrijemValidationResultDto> ValidatePrijemAsync(int rasporedId, decimal kolicina)
        {
            var raspored = await _ubranasirovinaRepository.GetRasporedbranjaForValidationAsync(rasporedId);

            var result = new PrijemValidationResultDto
            {
                Valid = true,
                Errors = new List<string>(),
                Warnings = new List<string>()
            };

            if (raspored == null)
            {
                result.Valid = false;
                result.Errors.Add("Raspored ne postoji");
                return result;
            }

            if (raspored.Ubranasirovina != null)
            {
                result.Valid = false;
                result.Errors.Add("Prijem za ovaj raspored već postoji!");
                return result;
            }

            var ukupnoUbrano = raspored.JeAngazovans.Sum(ja => ja.Kolicinaubrgr);
            result.UkupnoUbrano = ukupnoUbrano;

            if (kolicina <= 0)
            {
                result.Valid = false;
                result.Errors.Add("Količina mora biti veća od nule");
                return result;
            }

            if (kolicina > ukupnoUbrano)
            {
                result.Valid = false;
                result.Errors.Add($"Primljena količina ({kolicina:F2} kg) ne može biti veća od ubrane ({ukupnoUbrano:F2} kg)!");
                return result;
            }

            // Kalkulacija gubitaka
            result.Gubici = ukupnoUbrano - kolicina;
            result.PostotakGubitaka = ukupnoUbrano > 0
                ? (result.Gubici / ukupnoUbrano) * 100
                : 0;

            if (result.PostotakGubitaka > 15)
            {
                result.Valid = false;
                result.Errors.Add(
                    $"Gubici su preveliki: {result.PostotakGubitaka:F1}% (normalno < 15%). " +
                    $"Ubrano: {ukupnoUbrano:F2} kg, Primljeno: {kolicina:F2} kg"
                );
                return result;
            }

            if (result.PostotakGubitaka > 10)
            {
                result.Warnings.Add(
                    $"Upozorenje: Gubici su visoki ({result.PostotakGubitaka:F1}%). Provjerite da li je količina ispravno unesena."
                );
            }

            if (result.PostotakGubitaka < 1 && kolicina < ukupnoUbrano)
            {
                result.Warnings.Add(
                    $"Napomena: Gubici su izuzetno niski ({result.PostotakGubitaka:F1}%)."
                );
            }

            return result;
        }

        public async Task<UbranasirovinaDto> CreatePrijemAsync(CreateUbranasirovinaDto dto)
        {
            var validationResult = await ValidatePrijemAsync(dto.RasporedbranjaIdras, dto.Kolicina);

            if (!validationResult.Valid)
            {
                throw new InvalidOperationException(string.Join("; ", validationResult.Errors));
            }

            var prijem = new Ubranasirovina
            {
                Kolicina = dto.Kolicina,
                Datprijema = dto.Datprijema,
                RasporedbranjaIdras = dto.RasporedbranjaIdras
            };

            await _ubranasirovinaRepository.AddUbranasirovinaAsync(prijem);

            return (await GetPrijemByIdAsync(prijem.Idubrsir))!;
        }

        public async Task<bool> UpdatePrijemAsync(int id, UpdateUbranasirovinaDto dto)
        {
            var prijem = await _ubranasirovinaRepository.GetUbranasirovinaByIdAsync(id);

            if (prijem == null)
                return false;

            var validationResult = await ValidatePrijemAsync(prijem.RasporedbranjaIdras, dto.Kolicina);

            if (!validationResult.Valid)
            {
                throw new InvalidOperationException(string.Join("; ", validationResult.Errors));
            }

            prijem.Kolicina = dto.Kolicina;
            prijem.Datprijema = dto.Datprijema;

            await _ubranasirovinaRepository.UpdateUbranasirovinaAsync(prijem);
            return true;
        }

        public async Task<bool> DeletePrijemAsync(int id)
        {
            var prijem = await _ubranasirovinaRepository.GetUbranasirovinaByIdAsync(id);

            if (prijem == null)
                return false;

            await _ubranasirovinaRepository.DeleteUbranasirovinaAsync(prijem);
            return true;
        }

        private decimal CalculateStdDev(List<decimal> values)
        {
            if (!values.Any()) return 0;

            var avg = values.Average();
            var sumOfSquares = values.Sum(v => (double)Math.Pow((double)(v - avg), 2));
            return (decimal)Math.Sqrt(sumOfSquares / values.Count);
        }
    }
}


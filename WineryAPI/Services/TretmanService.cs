using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class TretmanService : ITretmanService
    {
        private readonly ITretmanRepository _tretmanRepository;

        public TretmanService(ITretmanRepository tretmanRepository)
        {
            _tretmanRepository = tretmanRepository;
        }

        public async Task<List<UbranasirovinaZaTretmanDto>> GetAllUbraneSirovineAsync()
        {
            var sirovine = await _tretmanRepository.GetAllUbraneSirovineAsync();

            var rezultat = sirovine.Select(us =>
            {
                var brojTretmana = us.Tretmen.Count;
                var aktivniTretmani = us.Tretmen.Count(t => t.Datzavresetkatret == null);
                
                decimal iskoriscenoKg = us.JeOsnovas.Sum(jo => jo.KolicinaGrozdja);
                decimal preostaloKg = us.Kolicina - iskoriscenoKg;
                int brojSirovihVina = us.JeOsnovas.Select(jo => jo.SirovovinoIdsirvina).Distinct().Count();

                string status;
                if (brojTretmana == 0)
                    status = "Nova";
                else if (aktivniTretmani > 0)
                    status = "UTretmanu";
                else if (preostaloKg <= 0)  // Sve grožđe iskorišćeno
                    status = "Obradjena";
                else
                    status = "SpremaZaVino";  // Tretmani završeni, ima još grožđa

                var seodrzava = us.RasporedbranjaIdrasNavigation?.Seodrzava;

                return new UbranasirovinaZaTretmanDto
                {
                    Idubrsir = us.Idubrsir,
                    Kolicina = us.Kolicina,
                    Datprijema = us.Datprijema.ToString("yyyy-MM-dd"),
                    BerbaNaziv = seodrzava?.BerbaIdberNavigation?.Nazber ?? "",
                    Sezona = seodrzava?.BerbaIdberNavigation?.Sezona ?? 0,
                    ParcelaNaziv = seodrzava?.ParcelaIdpNavigation?.Nazivparcele ?? "",
                    VinogradNaziv = seodrzava?.ParcelaIdpNavigation?.VinogradIdvNavigation?.Naziv ?? "",
                    SortaNaziv = seodrzava?.ParcelaIdpNavigation?.SortagrozdjaIdsrtNavigation?.Nazivsorte,
                    Status = status,
                    BrojTretmana = brojTretmana,
                    AktivniTretmani = aktivniTretmani,
                    PostojiSirovovino = brojSirovihVina > 0,
                    IskoriscenoKolicina = iskoriscenoKg,
                    PreostalaKolicina = preostaloKg,
                    BrojSirovihVina = brojSirovihVina
                };
            }).ToList();

            return rezultat;
        }

        public async Task<UbranasirovinaZaTretmanDto?> GetUbranaSirovinaByIdAsync(int id)
        {
            var us = await _tretmanRepository.GetUbranaSirovinaWithDetailsAsync(id);

            if (us == null)
                return null;

            var brojTretmana = us.Tretmen.Count;
            var aktivniTretmani = us.Tretmen.Count(t => t.Datzavresetkatret == null);
            
            decimal iskoriscenoKg = us.JeOsnovas.Sum(jo => jo.KolicinaGrozdja);
            decimal preostaloKg = us.Kolicina - iskoriscenoKg;
            int brojSirovihVina = us.JeOsnovas.Select(jo => jo.SirovovinoIdsirvina).Distinct().Count();

            string status;
            if (brojTretmana == 0)
                status = "Nova";
            else if (aktivniTretmani > 0)
                status = "UTretmanu";
            else if (preostaloKg <= 0)  // Sve grožđe iskorišćeno
                status = "Obradjena";
            else
                status = "SpremaZaVino";  // Tretmani završeni, ima još grožđa

            var seodrzava = us.RasporedbranjaIdrasNavigation?.Seodrzava;

            return new UbranasirovinaZaTretmanDto
            {
                Idubrsir = us.Idubrsir,
                Kolicina = us.Kolicina,
                Datprijema = us.Datprijema.ToString("yyyy-MM-dd"),
                BerbaNaziv = seodrzava?.BerbaIdberNavigation?.Nazber ?? "",
                Sezona = seodrzava?.BerbaIdberNavigation?.Sezona ?? 0,
                ParcelaNaziv = seodrzava?.ParcelaIdpNavigation?.Nazivparcele ?? "",
                VinogradNaziv = seodrzava?.ParcelaIdpNavigation?.VinogradIdvNavigation?.Naziv ?? "",
                SortaNaziv = seodrzava?.ParcelaIdpNavigation?.SortagrozdjaIdsrtNavigation?.Nazivsorte,
                Status = status,
                BrojTretmana = brojTretmana,
                AktivniTretmani = aktivniTretmani,
                PostojiSirovovino = brojSirovihVina > 0,
                IskoriscenoKolicina = iskoriscenoKg,
                PreostalaKolicina = preostaloKg,
                BrojSirovihVina = brojSirovihVina
            };
        }

        public async Task<List<TretmanDto>> GetTretmaniForUbranaSirovinaAsync(int ubranasirovinaId)
        {
            var tretmani = await _tretmanRepository.GetTretmaniByUbranaSirovinaAsync(ubranasirovinaId);

            return tretmani.Select(t => MapToTretmanDto(t)).ToList();
        }

        public async Task<TretmanDetailDto?> GetTretmanDetailAsync(int id)
        {
            var tretman = await _tretmanRepository.GetTretmanWithDetailsAsync(id);

            if (tretman == null)
                return null;

            var us = tretman.UbranasirovinaIdubrsirNavigation;
            var seodrzava = us?.RasporedbranjaIdrasNavigation?.Seodrzava;

            return new TretmanDetailDto
            {
                Idtretmana = tretman.Idtretmana,
                Naziv = tretman.Naziv,
                Datpocetkatret = tretman.Datpocetkatret.ToString("yyyy-MM-dd"),
                Datzavresetkatret = tretman.Datzavresetkatret?.ToString("yyyy-MM-dd"),
                TrajanjeUDanima = tretman.Datzavresetkatret.HasValue
                    ? (tretman.Datzavresetkatret.Value.ToDateTime(TimeOnly.MinValue) - tretman.Datpocetkatret.ToDateTime(TimeOnly.MinValue)).Days
                    : (DateTime.Now - tretman.Datpocetkatret.ToDateTime(TimeOnly.MinValue)).Days,
                JeAktivan = !tretman.Datzavresetkatret.HasValue,
                UbranaSirovina = new UbranasirovinaInfoDto
                {
                    Idubrsir = us?.Idubrsir ?? 0,
                    Kolicina = us?.Kolicina ?? 0,
                    BerbaNaziv = seodrzava?.BerbaIdberNavigation?.Nazber ?? "",
                    ParcelaNaziv = seodrzava?.ParcelaIdpNavigation?.Nazivparcele ?? "",
                    SortaNaziv = seodrzava?.ParcelaIdpNavigation?.SortagrozdjaIdsrtNavigation?.Nazivsorte
                },
                Sirovine = tretman.SeDodajes.Select(sd => new SirovinaUTretmanuDto
                {
                    SirovinazatretmanIdsir = sd.SirovinazatretmanIdsir,
                    NazivSirovine = sd.SirovinazatretmanIdsirNavigation.Naziv,
                    Kolicina = sd.Dodatakolicina
                }).ToList(),
                Enolozi = tretman.EnologIdzaps.Select(e => new EnologTretmanaDto
                {
                    Idzap = e.Idzap,
                    Ime = e.IdzapNavigation.Ime,
                    Prez = e.IdzapNavigation.Prez
                }).ToList()
            };
        }

        public async Task<TretmanDto> CreateTretmanAsync(CreateTretmanDto dto, int enologId)
        {
            var ubranaSirovina = await _tretmanRepository.GetUbranaSirovinaByIdAsync(dto.UbranasirovinaIdubrsir);
            if (ubranaSirovina == null)
                throw new KeyNotFoundException($"Ubrana sirovina sa ID {dto.UbranasirovinaIdubrsir} nije pronađena.");

            var datumPocetka = DateOnly.Parse(dto.Datpocetkatret);
            if (datumPocetka > DateOnly.FromDateTime(DateTime.Now))
                throw new InvalidOperationException("Datum početka ne može biti u budućnosti.");
            
            if (datumPocetka < ubranaSirovina.Datprijema)
                throw new InvalidOperationException("Tretman ne može početi pre datuma prijema ubrane sirovine.");

            var enolog = await _tretmanRepository.GetEnologByIdAsync(enologId);
            if (enolog == null)
                throw new KeyNotFoundException($"Enolog sa ID {enologId} nije pronađen.");

            var tretman = new Tretman
            {
                Naziv = dto.Naziv,
                Datpocetkatret = datumPocetka,
                UbranasirovinaIdubrsir = dto.UbranasirovinaIdubrsir
            };

            await _tretmanRepository.AddTretmanAsync(tretman);

            tretman.EnologIdzaps.Add(enolog);
            await _tretmanRepository.UpdateTretmanAsync(tretman);

            var kreiraniTretman = await GetTretmanDetailAsync(tretman.Idtretmana);
            
            return new TretmanDto
            {
                Idtretmana = kreiraniTretman!.Idtretmana,
                Naziv = kreiraniTretman.Naziv,
                Datpocetkatret = kreiraniTretman.Datpocetkatret,
                Datzavresetkatret = kreiraniTretman.Datzavresetkatret,
                TrajanjeUDanima = kreiraniTretman.TrajanjeUDanima,
                JeAktivan = kreiraniTretman.JeAktivan,
                UbranasirovinaIdubrsir = dto.UbranasirovinaIdubrsir,
                BrojSirovina = 0,
                Enolozi = kreiraniTretman.Enolozi
            };
        }

        public async Task CloseTretmanAsync(int id, CloseTretmanDto dto)
        {
            var tretman = await _tretmanRepository.GetTretmanWithSirovineAsync(id);

            if (tretman == null)
                throw new KeyNotFoundException($"Tretman sa ID {id} nije pronađen.");

            if (tretman.Datzavresetkatret.HasValue)
                throw new InvalidOperationException("Tretman je već zatvoren.");

            if (!tretman.SeDodajes.Any())
                throw new InvalidOperationException("Tretman mora imati bar jednu dodatu sirovinu prije zatvaranja.");

            var datumZavrsetka = DateOnly.Parse(dto.Datzavresetkatret);
            if (datumZavrsetka > DateOnly.FromDateTime(DateTime.Now))
                throw new InvalidOperationException("Datum završetka ne može biti u budućnosti.");

            if (datumZavrsetka < tretman.Datpocetkatret)
                throw new InvalidOperationException("Datum završetka ne može biti prije datuma početka tretmana.");

            tretman.Datzavresetkatret = datumZavrsetka;
            await _tretmanRepository.UpdateTretmanAsync(tretman);
        }

        public async Task AddSirovinaToTretmanAsync(int tretmanId, AddSirovinaToTretmanDto dto)
        {
            var tretman = await _tretmanRepository.GetTretmanWithSirovineAsync(tretmanId);

            if (tretman == null)
                throw new KeyNotFoundException($"Tretman sa ID {tretmanId} nije pronađen.");

            if (tretman.Datzavresetkatret.HasValue)
                throw new InvalidOperationException("Ne možete dodati sirovine u zatvoren tretman.");

            var sirovina = await _tretmanRepository.GetSirovinazatretmanByIdAsync(dto.SirovinazatretmanIdsir);
            if (sirovina == null)
                throw new KeyNotFoundException($"Sirovina sa ID {dto.SirovinazatretmanIdsir} nije pronađena.");

            var postojiSirovina = tretman.SeDodajes.Any(sd => sd.SirovinazatretmanIdsir == dto.SirovinazatretmanIdsir);
            if (postojiSirovina)
                throw new InvalidOperationException($"Sirovina '{sirovina.Naziv}' je već dodata u ovaj tretman.");

            if (dto.Kolicina <= 0)
                throw new InvalidOperationException("Količina mora biti veća od 0.");

            var seDodaje = new SeDodaje
            {
                TretmanIdtretmana = tretmanId,
                SirovinazatretmanIdsir = dto.SirovinazatretmanIdsir,
                Dodatakolicina = dto.Kolicina
            };

            await _tretmanRepository.AddSeDodajeAsync(seDodaje);
        }

        public async Task RemoveSirovinaFromTretmanAsync(int tretmanId, int sirovinazatretmanIdsir)
        {
            var tretman = await _tretmanRepository.GetTretmanWithSirovineAsync(tretmanId);

            if (tretman == null)
                throw new KeyNotFoundException($"Tretman sa ID {tretmanId} nije pronađen.");

            if (tretman.Datzavresetkatret.HasValue)
                throw new InvalidOperationException("Ne možete ukloniti sirovine iz zatvorenog tretmana.");

            var seDodaje = await _tretmanRepository.GetSeDodajeAsync(tretmanId, sirovinazatretmanIdsir);
            if (seDodaje == null)
                throw new KeyNotFoundException("Sirovina nije pronađena u ovom tretmanu.");

            await _tretmanRepository.DeleteSeDodajeAsync(seDodaje);
        }

        public async Task<List<TretmanDto>> GetAktivniTretmaniAsync()
        {
            var tretmani = await _tretmanRepository.GetAktivniTretmaniAsync();

            return tretmani.Select(t => MapToTretmanDto(t)).ToList();
        }

        public async Task<List<TretmanDto>> GetZavreniTretmaniAsync()
        {
            var tretmani = await _tretmanRepository.GetZavreniTretmaniAsync();

            return tretmani.Select(t => MapToTretmanDto(t)).ToList();
        }

        private TretmanDto MapToTretmanDto(Tretman tretman)
        {
            return new TretmanDto
            {
                Idtretmana = tretman.Idtretmana,
                Naziv = tretman.Naziv,
                Datpocetkatret = tretman.Datpocetkatret.ToString("yyyy-MM-dd"),
                Datzavresetkatret = tretman.Datzavresetkatret?.ToString("yyyy-MM-dd"),
                TrajanjeUDanima = tretman.Datzavresetkatret.HasValue
                    ? (tretman.Datzavresetkatret.Value.ToDateTime(TimeOnly.MinValue) - tretman.Datpocetkatret.ToDateTime(TimeOnly.MinValue)).Days
                    : (DateTime.Now - tretman.Datpocetkatret.ToDateTime(TimeOnly.MinValue)).Days,
                JeAktivan = !tretman.Datzavresetkatret.HasValue,
                UbranasirovinaIdubrsir = tretman.UbranasirovinaIdubrsir,
                BrojSirovina = tretman.SeDodajes.Count,
                Enolozi = tretman.EnologIdzaps.Select(e => new EnologTretmanaDto
                {
                    Idzap = e.Idzap,
                    Ime = e.IdzapNavigation.Ime,
                    Prez = e.IdzapNavigation.Prez
                }).ToList()
            };
        }
    }
}


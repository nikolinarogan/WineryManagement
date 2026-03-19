using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class BerbaService : IBerbaService
    {
        private readonly IBerbaRepository _berbaRepository;
        private readonly IRadoviRepository _radoviRepository;

        public BerbaService(IBerbaRepository berbaRepository, IRadoviRepository radoviRepository)
        {
            _berbaRepository = berbaRepository;
            _radoviRepository = radoviRepository;
        }

        public async Task<List<BerbaDto>> GetAllBerbeAsync()
        {
            var berbe = await _berbaRepository.GetAllBerbeAsync();

            return berbe.Select(b => new BerbaDto
            {
                Idber = b.Idber,
                Nazber = b.Nazber,
                Sezona = b.Sezona,
                BrojParcela = b.Seodrzavas.Count
            }).ToList();
        }

        public async Task<BerbaDetailDto?> GetBerbaByIdAsync(int id)
        {
            var berba = await _berbaRepository.GetBerbaWithDetailsAsync(id);

            if (berba == null)
                return null;

            return new BerbaDetailDto
            {
                Idber = berba.Idber,
                Nazber = berba.Nazber,
                Sezona = berba.Sezona,
                Parcele = berba.Seodrzavas.Select(s => new ParcelaBerbaDto
                {
                    ParcelaIdp = s.ParcelaIdp,
                    Nazivparcele = s.ParcelaIdpNavigation.Nazivparcele,
                    VinogradNaziv = s.ParcelaIdpNavigation.VinogradIdvNavigation.Naziv,
                    SortaNaziv = s.ParcelaIdpNavigation.SortagrozdjaIdsrtNavigation?.Nazivsorte,
                    Povrsina = s.ParcelaIdpNavigation.Povrsina,
                    Brojcokota = s.ParcelaIdpNavigation.Brojcokota
                }).ToList()
            };
        }

        public async Task<BerbaDto> CreateBerbaAsync(CreateBerbaDto dto)
        {
            var berba = new Berba
            {
                Nazber = dto.Nazber,
                Sezona = dto.Sezona
            };

            berba = await _berbaRepository.AddBerbaAsync(berba);

            // Dodaj parcele ako su proslijeđene
            if (dto.ParcelaIds != null && dto.ParcelaIds.Any())
            {
                foreach (var parcelaId in dto.ParcelaIds)
                {
                    var seodrzava = new Seodrzava
                    {
                        BerbaIdber = berba.Idber,
                        ParcelaIdp = parcelaId
                    };
                    await _berbaRepository.AddSeodrzavaAsync(seodrzava);
                }
            }

            return new BerbaDto
            {
                Idber = berba.Idber,
                Nazber = berba.Nazber,
                Sezona = berba.Sezona,
                BrojParcela = dto.ParcelaIds?.Count ?? 0
            };
        }

        public async Task<bool> UpdateBerbaAsync(int id, UpdateBerbaDto dto)
        {
            var berba = await _berbaRepository.GetBerbaByIdAsync(id);
            if (berba == null)
                return false;

            berba.Nazber = dto.Nazber;
            berba.Sezona = dto.Sezona;

            await _berbaRepository.UpdateBerbaAsync(berba);
            return true;
        }

        public async Task<bool> DeleteBerbaAsync(int id)
        {
            var berba = await _berbaRepository.GetBerbaByIdAsync(id);
            if (berba == null)
                return false;

            await _berbaRepository.DeleteBerbaAsync(berba);
            return true;
        }

        public async Task<bool> AddParcelaToBerbaAsync(int berbaId, int parcelaId)
        {
            var berba = await _berbaRepository.GetBerbaByIdAsync(berbaId);
            if (berba == null)
                return false;

            var parcela = await _berbaRepository.GetParcelaByIdAsync(parcelaId);
            if (parcela == null)
                return false;

            var postojeca = await _berbaRepository.GetSeodrzavaAsync(berbaId, parcelaId);

            if (postojeca != null)
                return false; 

            var seodrzava = new Seodrzava
            {
                BerbaIdber = berbaId,
                ParcelaIdp = parcelaId
            };

            await _berbaRepository.AddSeodrzavaAsync(seodrzava);
            return true;
        }

        public async Task<bool> RemoveParcelaFromBerbaAsync(int berbaId, int parcelaId)
        {
            var seodrzava = await _berbaRepository.GetSeodrzavaAsync(berbaId, parcelaId);

            if (seodrzava == null)
                return false;

            await _berbaRepository.DeleteSeodrzavaAsync(seodrzava);
            return true;
        }

        // ==================== RasporedBranja  ====================

        public async Task<List<RasporedbranjaDto>> GetAllRasporedbranjaAsync(int? berbaId = null)
        {
            var rasporedi = await _berbaRepository.GetAllRasporedbranjaAsync(berbaId);

            return rasporedi.Select(r => new RasporedbranjaDto
            {
                Idras = r.Idras,
                Pocbranja = r.Pocbranja,
                Zavrsetakbranja = r.Zavrsetakbranja,
                Ocekivaniprinos = r.Ocekivaniprinos,
                MenadzerIdzap = r.MenadzerIdzap,
                MenadzerIme = r.MenadzerIdzapNavigation.IdzapNavigation.Ime,
                MenadzerPrezime = r.MenadzerIdzapNavigation.IdzapNavigation.Prez,
                SeodrzavaIdber = r.SeodrzavaIdber,
                BerbaNaziv = r.Seodrzava.BerbaIdberNavigation.Nazber,
                SeodrzavaIdp = r.SeodrzavaIdp,
                ParcelaNaziv = r.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                BrojRadnika = r.JeAngazovans.Count,
                BrojRadnikaSaUnosom = r.JeAngazovans.Count(ja => ja.Kolicinaubrgr > 0), 
                UkupnoUbrano = r.JeAngazovans.Sum(ja => ja.Kolicinaubrgr)
            }).ToList();
        }

        public async Task<RasporedbranjaDetailDto?> GetRasporedbranjaByIdAsync(int id)
        {
            var raspored = await _berbaRepository.GetRasporedbranjaWithDetailsAsync(id);

            if (raspored == null)
                return null;

            return new RasporedbranjaDetailDto
            {
                Idras = raspored.Idras,
                Pocbranja = raspored.Pocbranja,
                Zavrsetakbranja = raspored.Zavrsetakbranja,
                Ocekivaniprinos = raspored.Ocekivaniprinos,
                MenadzerIdzap = raspored.MenadzerIdzap,
                MenadzerIme = raspored.MenadzerIdzapNavigation.IdzapNavigation.Ime,
                MenadzerPrezime = raspored.MenadzerIdzapNavigation.IdzapNavigation.Prez,
                SeodrzavaIdber = raspored.SeodrzavaIdber,
                BerbaNaziv = raspored.Seodrzava.BerbaIdberNavigation.Nazber,
                SeodrzavaIdp = raspored.SeodrzavaIdp,
                ParcelaNaziv = raspored.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                VinogradNaziv = raspored.Seodrzava.ParcelaIdpNavigation.VinogradIdvNavigation.Naziv,
                Radnici = raspored.JeAngazovans.Select(ja => new RadnikNaRasporedDto
                {
                    RadnikIdzap = ja.RadnikIdzap,
                    Ime = ja.RadnikIdzapNavigation.IdzapNavigation.Ime,
                    Prezime = ja.RadnikIdzapNavigation.IdzapNavigation.Prez,
                    Kolicinaubrgr = ja.Kolicinaubrgr,
                    Datumbranja = ja.Datumbranja
                }).ToList()
            };
        }

        public async Task<RasporedbranjaDto> CreateRasporedbranjaAsync(CreateRasporedbranjaDto dto)
        {
            // Provjera da li postoji seOdrzava za ovu berbu i parcelu
            var seOdrzava = await _berbaRepository.GetSeodrzavaAsync(dto.BerbaIdber, dto.ParcelaIdp);

            if (seOdrzava == null)
                throw new InvalidOperationException("Parcela nije dio navedene berbe");

            var raspored = new Rasporedbranja
            {
                Pocbranja = dto.Pocbranja,
                Zavrsetakbranja = dto.Zavrsetakbranja,
                Ocekivaniprinos = dto.Ocekivaniprinos,
                MenadzerIdzap = dto.MenadzerIdzap,
                SeodrzavaIdber = dto.BerbaIdber,
                SeodrzavaIdp = dto.ParcelaIdp
            };

            raspored = await _berbaRepository.AddRasporedbranjaAsync(raspored);

            var created = await _berbaRepository.GetRasporedbranjaWithDetailsAsync(raspored.Idras);

            return new RasporedbranjaDto
            {
                Idras = created!.Idras,
                Pocbranja = created.Pocbranja,
                Zavrsetakbranja = created.Zavrsetakbranja,
                Ocekivaniprinos = created.Ocekivaniprinos,
                MenadzerIdzap = created.MenadzerIdzap,
                MenadzerIme = created.MenadzerIdzapNavigation.IdzapNavigation.Ime,
                MenadzerPrezime = created.MenadzerIdzapNavigation.IdzapNavigation.Prez,
                SeodrzavaIdber = created.SeodrzavaIdber,
                BerbaNaziv = created.Seodrzava.BerbaIdberNavigation.Nazber,
                SeodrzavaIdp = created.SeodrzavaIdp,
                ParcelaNaziv = created.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                BrojRadnika = 0,
                BrojRadnikaSaUnosom = 0,
                UkupnoUbrano = 0
            };
        }

        public async Task<bool> UpdateRasporedbranjaAsync(int id, UpdateRasporedbranjaDto dto)
        {
            var raspored = await _berbaRepository.GetRasporedbranjaByIdAsync(id);
            if (raspored == null)
                return false;

            raspored.Pocbranja = dto.Pocbranja;
            raspored.Zavrsetakbranja = dto.Zavrsetakbranja;
            raspored.Ocekivaniprinos = dto.Ocekivaniprinos;

            await _berbaRepository.UpdateRasporedbranjaAsync(raspored);
            return true;
        }

        public async Task<bool> DeleteRasporedbranjaAsync(int id)
        {
            var raspored = await _berbaRepository.GetRasporedbranjaByIdAsync(id);
            if (raspored == null)
                return false;

            await _berbaRepository.DeleteRasporedbranjaAsync(raspored);
            return true;
        }

        public async Task<bool> AddRadnikToRasporedAsync(int rasporedId, AddRadnikToRasporedDto dto)
        {
            var raspored = await _berbaRepository.GetRasporedbranjaWithDetailsAsync(rasporedId);
            
            if (raspored == null)
                return false;

            var radnik = await _berbaRepository.GetRadnikByIdAsync(dto.RadnikIdzap);
            if (radnik == null)
                return false;

            var alreadyAssigned = await _berbaRepository.GetJeAngazovanAsync(rasporedId, dto.RadnikIdzap);

            if (alreadyAssigned != null)
                throw new InvalidOperationException("Radnik je već dodijeljen ovom rasporedu");

            var pocetakNovog = raspored.Pocbranja;
            var zavrsetakNovog = raspored.Zavrsetakbranja;

            var radnikRasporedi = await _berbaRepository.GetJeAngazovanByRadnikAsync(dto.RadnikIdzap);

            var preklapajuciRaspored = radnikRasporedi
                .Where(ja => 
                    ja.RasporedbranjaIdrasNavigation.Pocbranja <= zavrsetakNovog &&
                    ja.RasporedbranjaIdrasNavigation.Zavrsetakbranja >= pocetakNovog
                )
                .FirstOrDefault();

            if (preklapajuciRaspored != null)
            {
                var berba = preklapajuciRaspored.RasporedbranjaIdrasNavigation.Seodrzava.BerbaIdberNavigation;
                var parcela = preklapajuciRaspored.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation;
                throw new InvalidOperationException(
                    $"Radnik već ima zakazan raspored u periodu od {preklapajuciRaspored.RasporedbranjaIdrasNavigation.Pocbranja:dd.MM.yyyy} " +
                    $"do {preklapajuciRaspored.RasporedbranjaIdrasNavigation.Zavrsetakbranja:dd.MM.yyyy} " +
                    $"za berbu '{berba.Nazber}' na parceli '{parcela.Nazivparcele}'"
                );
            }

            var preklapajuciRadovi = await _radoviRepository.GetRadoviForRadnikInDateRangeAsync(
                dto.RadnikIdzap,
                pocetakNovog,
                zavrsetakNovog
            );

            if (preklapajuciRadovi.Any())
            {
                var rad = preklapajuciRadovi.First();
                var parcelaNames = string.Join(", ", rad.ParcelaIdps.Take(2).Select(p => p.Nazivparcele));
                throw new InvalidOperationException(
                    $"Radnik već ima zakazan rad u periodu od {rad.Pocrad:dd.MM.yyyy} " +
                    $"do {rad.Zavrrad:dd.MM.yyyy} na parcelama: {parcelaNames}"
                );
            }

            var jeAngazovan = new JeAngazovan
            {
                RasporedbranjaIdras = rasporedId,
                RadnikIdzap = dto.RadnikIdzap,
                Kolicinaubrgr = dto.Kolicinaubrgr,
                Datumbranja = dto.Datumbranja
            };

            await _berbaRepository.AddJeAngazovanAsync(jeAngazovan);
            return true;
        }

        public async Task<bool> RemoveRadnikFromRasporedAsync(int rasporedId, int radnikId)
        {
            var jeAngazovan = await _berbaRepository.GetJeAngazovanAsync(rasporedId, radnikId);

            if (jeAngazovan == null)
                return false;

            await _berbaRepository.DeleteJeAngazovanAsync(jeAngazovan);
            return true;
        }

        public async Task<List<RasporedbranjaDto>> GetRasporediForRadnikAsync(int radnikId)
        {
            var angazovanja = await _berbaRepository.GetJeAngazovanByRadnikAsync(radnikId);

            var rasporedi = angazovanja.Select(ja => ja.RasporedbranjaIdrasNavigation).Distinct().ToList();

            return rasporedi.Select(r => new RasporedbranjaDto
            {
                Idras = r.Idras,
                Pocbranja = r.Pocbranja,
                Zavrsetakbranja = r.Zavrsetakbranja,
                Ocekivaniprinos = r.Ocekivaniprinos,
                MenadzerIdzap = r.MenadzerIdzap,
                MenadzerIme = r.MenadzerIdzapNavigation.IdzapNavigation.Ime,
                MenadzerPrezime = r.MenadzerIdzapNavigation.IdzapNavigation.Prez,
                SeodrzavaIdber = r.SeodrzavaIdber,
                BerbaNaziv = r.Seodrzava.BerbaIdberNavigation.Nazber,
                SeodrzavaIdp = r.SeodrzavaIdp,
                ParcelaNaziv = r.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                BrojRadnika = r.JeAngazovans.Count,
                BrojRadnikaSaUnosom = r.JeAngazovans.Count(ja => ja.Kolicinaubrgr > 0), 
                UkupnoUbrano = r.JeAngazovans.Sum(ja => ja.Kolicinaubrgr)
            }).ToList();
        }

        public async Task<List<RadnikRasporedDto>> GetMojiRasporediDetaljiAsync(int radnikId)
        {
            var angazovanja = await _berbaRepository.GetJeAngazovanByRadnikAsync(radnikId);

            return angazovanja.Select(ja => new RadnikRasporedDto
            {
                Idras = ja.RasporedbranjaIdras,
                BerbaNaziv = ja.RasporedbranjaIdrasNavigation.Seodrzava.BerbaIdberNavigation.Nazber,
                ParcelaNaziv = ja.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.Nazivparcele,
                VinogradNaziv = ja.RasporedbranjaIdrasNavigation.Seodrzava.ParcelaIdpNavigation.VinogradIdvNavigation.Naziv,
                Pocbranja = ja.RasporedbranjaIdrasNavigation.Pocbranja,
                Zavrsetakbranja = ja.RasporedbranjaIdrasNavigation.Zavrsetakbranja,
                MojaKolicina = ja.Kolicinaubrgr,
                MojDatumBranja = ja.Datumbranja,
                MenadzerIme = ja.RasporedbranjaIdrasNavigation.MenadzerIdzapNavigation.IdzapNavigation.Ime,
                MenadzerPrezime = ja.RasporedbranjaIdrasNavigation.MenadzerIdzapNavigation.IdzapNavigation.Prez
            }).ToList();
        }

        public async Task<bool> UpdateMojaKolicinaAsync(int radnikId, int rasporedId, UpdateRadnikKolicinaDto dto)
        {
            var angazovanje = await _berbaRepository.GetJeAngazovanAsync(rasporedId, radnikId);

            if (angazovanje == null)
                return false;

            var raspored = await _berbaRepository.GetRasporedbranjaByIdAsync(rasporedId);
            if (raspored == null)
                return false;

            var danas = DateOnly.FromDateTime(DateTime.Now);
            if (danas < raspored.Pocbranja || danas > raspored.Zavrsetakbranja)
            {
                throw new InvalidOperationException(
                    $"Ne možete uneti učinak. Raspored se odvija u periodu od {raspored.Pocbranja:dd.MM.yyyy} " +
                    $"do {raspored.Zavrsetakbranja:dd.MM.yyyy}. Današnji datum je {danas:dd.MM.yyyy}."
                );
            }

            angazovanje.Kolicinaubrgr = dto.Kolicinaubrgr;
            angazovanje.Datumbranja = dto.Datumbranja;

            await _berbaRepository.UpdateJeAngazovanAsync(angazovanje);
            return true;
        }

        public async Task<BerbaStatistikaDto?> GetBerbaStatistikaAsync(int berbaId)
        {
            var berba = await _berbaRepository.GetBerbaWithDetailsAsync(berbaId);

            if (berba == null)
                return null;

            var sviRasporedi = await _berbaRepository.GetRasporedbranjaByBerbaIdAsync(berbaId);
            var svaAngazovanja = sviRasporedi.SelectMany(r => r.JeAngazovans).ToList();

            var ukupnoUbrano = svaAngazovanja.Sum(ja => ja.Kolicinaubrgr);
            var ocekivanoUkupno = sviRasporedi.Sum(r => r.Ocekivaniprinos);
            var procenatRealizacije = ocekivanoUkupno > 0 ? (ukupnoUbrano / ocekivanoUkupno) * 100 : 0;

            var radniciUcinak = svaAngazovanja
                .GroupBy(ja => new { ja.RadnikIdzap, ja.RasporedbranjaIdras })
                .Select(g =>
                {
                    var firstItem = g.First();
                    var raspored = sviRasporedi.First(r => r.Idras == g.Key.RasporedbranjaIdras);
                    var seodrzava = berba.Seodrzavas.FirstOrDefault(s => 
                        s.BerbaIdber == raspored.SeodrzavaIdber && s.ParcelaIdp == raspored.SeodrzavaIdp);
                    
                    return new RadnikUcinakDto
                    {
                        RadnikId = g.Key.RadnikIdzap,
                        RadnikIme = firstItem.RadnikIdzapNavigation.IdzapNavigation.Ime,
                        RadnikPrezime = firstItem.RadnikIdzapNavigation.IdzapNavigation.Prez,
                        Kolicina = g.Sum(x => x.Kolicinaubrgr),
                        DatumBranja = firstItem.Datumbranja,
                        ParcelaNaziv = seodrzava?.ParcelaIdpNavigation?.Nazivparcele ?? "N/A"
                    };
                })
                .OrderByDescending(r => r.Kolicina)
                .ToList();

            return new BerbaStatistikaDto
            {
                BerbaIdber = berba.Idber,
                BerbaNaziv = berba.Nazber,
                Sezona = berba.Sezona,
                UkupnoUbrano = ukupnoUbrano,
                OcekivanoUkupno = ocekivanoUkupno,
                ProcenatRealizacije = procenatRealizacije,
                BrojRadnika = svaAngazovanja.Select(ja => ja.RadnikIdzap).Distinct().Count(),
                BrojRasporeda = sviRasporedi.Count,
                RadniciUcinak = radniciUcinak
            };
        }
    }
}


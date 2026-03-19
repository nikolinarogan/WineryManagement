using Microsoft.EntityFrameworkCore;
using WineryAPI.Data;
using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class SirovovinoService : ISirovovinoService
    {
        private readonly ISirovovinoRepository _sirovovinoRepository;
        private readonly IUbranasirovinaRepository _ubranasirovinaRepository;
        private readonly ITretmanRepository _tretmanRepository;

        public SirovovinoService(
            ISirovovinoRepository sirovovinoRepository,
            IUbranasirovinaRepository ubranasirovinaRepository,
            ITretmanRepository tretmanRepository)
        {
            _sirovovinoRepository = sirovovinoRepository;
            _ubranasirovinaRepository = ubranasirovinaRepository;
            _tretmanRepository = tretmanRepository;
        }

        public async Task<List<SirovovinoDto>> GetAllSirovovinaAsync()
        {
            var sirovinaVina = await _sirovovinoRepository.GetAllSirovovinaAsync();

            return sirovinaVina.Select(sv => MapToSirovovinoDto(sv)).ToList();
        }

        public async Task<SirovovinoDto?> GetSirovovinoByIdAsync(int id)
        {
            var sirovovino = await _sirovovinoRepository.GetSirovovinoWithDetailsAsync(id);

            if (sirovovino == null)
                return null;

            return MapToSirovovinoDto(sirovovino);
        }

        public async Task<SirovovinoDto> CreateSirovovinoAsync(CreateSirovovinoDto dto)
        {
            if (dto.UbraneSirovine == null || !dto.UbraneSirovine.Any())
            {
                throw new InvalidOperationException("Mora se uneti bar jedna ubrana sirovina sa količinom grožđa.");
            }

            if (dto.Kolicinasirvina <= 0)
                throw new InvalidOperationException("Količina sirovog vina mora biti veća od 0.");

            var trenutnaGodina = DateTime.Now.Year;
            if (dto.Godproizvodnje > trenutnaGodina)
                throw new InvalidOperationException($"Godina proizvodnje ne može biti u budućnosti (max {trenutnaGodina}).");

            foreach (var input in dto.UbraneSirovine)
            {
                if (input.KolicinaGrozdja <= 0)
                    throw new InvalidOperationException($"Količina grožđa mora biti veća od 0 za ubranu sirovinu ID {input.UbranasirovinaId}.");
            }

            foreach (var input in dto.UbraneSirovine)
            {
                var ubranaSirovina = await _ubranasirovinaRepository.GetUbranasirovinaByIdAsync(input.UbranasirovinaId);
                if (ubranaSirovina == null)
                    throw new KeyNotFoundException($"Ubrana sirovina sa ID {input.UbranasirovinaId} nije pronađena.");

                var tretmani = await _tretmanRepository.GetTretmaniByUbranaSirovinaAsync(input.UbranasirovinaId);
                if (!tretmani.Any())
                {
                    throw new InvalidOperationException(
                        $"Ubrana sirovina ID {input.UbranasirovinaId} mora proći kroz bar jedan tretman prije kreiranja sirovog vina.");
                }

                var aktivniTretmani = tretmani.Count(t => t.Datzavresetkatret == null);
                if (aktivniTretmani > 0)
                {
                    throw new InvalidOperationException(
                        $"Ubrana sirovina ID {input.UbranasirovinaId} ima {aktivniTretmani} aktivnih tretmana. Zatvorite sve tretmane prije kreiranja sirovog vina.");
                }

                var iskoriscenoKg = await _sirovovinoRepository.GetIskoriscenoGrozdje(input.UbranasirovinaId);
                var preostaloKg = ubranaSirovina.Kolicina - iskoriscenoKg;

                if (input.KolicinaGrozdja > preostaloKg)
                {
                    throw new InvalidOperationException(
                        $"Nedovoljno grožđa! Ubrana sirovina ID {input.UbranasirovinaId}: Traženo {input.KolicinaGrozdja} kg, Preostalo {preostaloKg} kg.");
                }
            }

            var sirovovino = new Sirovovino
            {
                Nazivsirvina = dto.Nazivsirvina,
                Kolicinasirvina = dto.Kolicinasirvina,
                Kvalitet = dto.Kvalitet,
                Godproizvodnje = dto.Godproizvodnje
            };

            await _sirovovinoRepository.AddSirovovinoAsync(sirovovino);

            foreach (var input in dto.UbraneSirovine)
            {
                var jeOsnova = new JeOsnova
                {
                    UbranasirovinaIdubrsir = input.UbranasirovinaId,
                    SirovovinoIdsirvina = sirovovino.Idsirvina,
                    KolicinaGrozdja = input.KolicinaGrozdja
                };
                await _sirovovinoRepository.AddJeOsnovaAsync(jeOsnova);
            }

            return await GetSirovovinoByIdAsync(sirovovino.Idsirvina) 
                ?? throw new InvalidOperationException("Greška pri kreiranju sirovog vina.");
        }

        public async Task UpdateSirovovinoAsync(int id, UpdateSirovovinoDto dto)
        {
            var sirovovino = await _sirovovinoRepository.GetSirovovinoByIdAsync(id);

            if (sirovovino == null)
                throw new KeyNotFoundException($"Sirovo vino sa ID {id} nije pronađeno.");

            if (dto.Kolicinasirvina <= 0)
                throw new InvalidOperationException("Količina sirovog vina mora biti veća od 0.");

            sirovovino.Nazivsirvina = dto.Nazivsirvina;
            sirovovino.Kolicinasirvina = dto.Kolicinasirvina;
            sirovovino.Kvalitet = dto.Kvalitet;

            await _sirovovinoRepository.UpdateSirovovinoAsync(sirovovino);
        }

        public async Task DeleteSirovovinoAsync(int id)
        {
            var sirovovino = await _sirovovinoRepository.GetSirovovinoForDeletionAsync(id);

            if (sirovovino == null)
                throw new KeyNotFoundException($"Sirovo vino sa ID {id} nije pronađeno.");

            if (sirovovino.SeLagerujes.Any())
            {
                throw new InvalidOperationException(
                    $"Sirovo vino '{sirovovino.Nazivsirvina}' ne može biti obrisano jer je lagerovano u {sirovovino.SeLagerujes.Count} buradi.");
            }

            if (sirovovino.VinoIdvinas.Any())
            {
                throw new InvalidOperationException(
                    $"Sirovo vino '{sirovovino.Nazivsirvina}' ne može biti obrisano jer je korišćeno za blending u {sirovovino.VinoIdvinas.Count} finalnih vina.");
            }

            await _sirovovinoRepository.DeleteSirovovinoAsync(sirovovino);
        }

        private SirovovinoDto MapToSirovovinoDto(Sirovovino sv)
        {
            return new SirovovinoDto
            {
                Idsirvina = sv.Idsirvina,
                Nazivsirvina = sv.Nazivsirvina,
                Kolicinasirvina = sv.Kolicinasirvina,
                Kvalitet = sv.Kvalitet,
                Godproizvodnje = sv.Godproizvodnje,
                Poreklo = sv.JeOsnovas.Select(jo =>  
                {
                    var us = jo.UbranasirovinaIdubrsirNavigation;
                    var seodrzava = us?.RasporedbranjaIdrasNavigation?.Seodrzava;
                    return new PorekloSirovinaDto
                    {
                        UbranasirovinaId = us?.Idubrsir ?? 0,
                        BerbaNaziv = seodrzava?.BerbaIdberNavigation?.Nazber ?? "",
                        ParcelaNaziv = seodrzava?.ParcelaIdpNavigation?.Nazivparcele ?? "",
                        SortaNaziv = seodrzava?.ParcelaIdpNavigation?.SortagrozdjaIdsrtNavigation?.Nazivsorte,
                        BrojTretmana = us?.Tretmen.Count ?? 0,
                        KolicinaGrozdja = jo.KolicinaGrozdja  
                    };
                }).ToList()
            };
        }
    }
}


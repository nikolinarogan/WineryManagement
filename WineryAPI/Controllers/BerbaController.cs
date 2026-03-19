using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class BerbaController : ControllerBase
    {
        private readonly IBerbaService _berbaService;

        public BerbaController(IBerbaService berbaService)
        {
            _berbaService = berbaService;
        }

        [HttpGet]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetAllBerbe()
        {
            try
            {
                var berbe = await _berbaService.GetAllBerbeAsync();
                return Ok(berbe);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetBerbaById(int id)
        {
            try
            {
                var berba = await _berbaService.GetBerbaByIdAsync(id);
                if (berba == null)
                    return NotFound(new { message = "Berba nije pronađena" });

                return Ok(berba);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> CreateBerba([FromBody] CreateBerbaDto dto)
        {
            try
            {
                var berba = await _berbaService.CreateBerbaAsync(dto);
                return CreatedAtAction(nameof(GetBerbaById), new { id = berba.Idber }, berba);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> UpdateBerba(int id, [FromBody] UpdateBerbaDto dto)
        {
            try
            {
                var success = await _berbaService.UpdateBerbaAsync(id, dto);
                if (!success)
                    return NotFound(new { message = "Berba nije pronađena" });

                return Ok(new { message = "Berba uspješno ažurirana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> DeleteBerba(int id)
        {
            try
            {
                var success = await _berbaService.DeleteBerbaAsync(id);
                if (!success)
                    return NotFound(new { message = "Berba nije pronađena" });

                return Ok(new { message = "Berba uspješno obrisana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{berbaId}/parcele/{parcelaId}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> AddParcelaToBerba(int berbaId, int parcelaId)
        {
            try
            {
                var success = await _berbaService.AddParcelaToBerbaAsync(berbaId, parcelaId);
                if (!success)
                    return BadRequest(new { message = "Parcela već postoji u berbi ili nije pronađena" });

                return Ok(new { message = "Parcela uspješno dodata u berbu" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{berbaId}/parcele/{parcelaId}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> RemoveParcelaFromBerba(int berbaId, int parcelaId)
        {
            try
            {
                var success = await _berbaService.RemoveParcelaFromBerbaAsync(berbaId, parcelaId);
                if (!success)
                    return NotFound(new { message = "Parcela nije pronađena u berbi" });

                return Ok(new { message = "Parcela uspješno uklonjena iz berbe" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==================== RasporedBranja  ====================

        [HttpGet("rasporedi")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetAllRasporedi([FromQuery] int? berbaId = null)
        {
            try
            {
                var rasporedi = await _berbaService.GetAllRasporedbranjaAsync(berbaId);
                return Ok(rasporedi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("rasporedi/{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetRasporedById(int id)
        {
            try
            {
                var raspored = await _berbaService.GetRasporedbranjaByIdAsync(id);
                if (raspored == null)
                    return NotFound(new { message = "Raspored nije pronađen" });

                return Ok(raspored);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("rasporedi")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> CreateRaspored([FromBody] CreateRasporedbranjaDto dto)
        {
            try
            {
                var raspored = await _berbaService.CreateRasporedbranjaAsync(dto);
                return CreatedAtAction(nameof(GetRasporedById), new { id = raspored.Idras }, raspored);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("rasporedi/{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> UpdateRaspored(int id, [FromBody] UpdateRasporedbranjaDto dto)
        {
            try
            {
                var success = await _berbaService.UpdateRasporedbranjaAsync(id, dto);
                if (!success)
                    return NotFound(new { message = "Raspored nije pronađen" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("rasporedi/{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> DeleteRaspored(int id)
        {
            try
            {
                var success = await _berbaService.DeleteRasporedbranjaAsync(id);
                if (!success)
                    return NotFound(new { message = "Raspored nije pronađen" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("rasporedi/{rasporedId}/radnici")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> AddRadnikToRaspored(int rasporedId, [FromBody] AddRadnikToRasporedDto dto)
        {
            try
            {
                var success = await _berbaService.AddRadnikToRasporedAsync(rasporedId, dto);
                if (!success)
                    return NotFound(new { message = "Raspored ili radnik nisu pronađeni" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("rasporedi/{rasporedId}/radnici/{radnikId}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> RemoveRadnikFromRaspored(int rasporedId, int radnikId)
        {
            try
            {
                var success = await _berbaService.RemoveRadnikFromRasporedAsync(rasporedId, radnikId);
                if (!success)
                    return NotFound(new { message = "Radnik nije pronađen na navedenom rasporedu" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("moji-rasporedi")]
        [Authorize(Roles = "Radnik")]
        public async Task<IActionResult> GetMojiRasporedi()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var rasporedi = await _berbaService.GetRasporediForRadnikAsync(userId);
                return Ok(rasporedi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("moji-rasporedi/detalji")]
        [Authorize(Roles = "Radnik")]
        public async Task<IActionResult> GetMojiRasporediDetalji()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var rasporedi = await _berbaService.GetMojiRasporediDetaljiAsync(userId);
                return Ok(rasporedi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("moji-rasporedi/{rasporedId}/kolicina")]
        [Authorize(Roles = "Radnik")]
        public async Task<IActionResult> UpdateMojaKolicina(int rasporedId, [FromBody] UpdateRadnikKolicinaDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var success = await _berbaService.UpdateMojaKolicinaAsync(userId, rasporedId, dto);
                
                if (!success)
                    return NotFound(new { message = "Niste angažovani na navedenom rasporedu" });

                return Ok(new { message = "Količina uspješno ažurirana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{berbaId}/statistika")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetBerbaStatistika(int berbaId)
        {
            try
            {
                var statistika = await _berbaService.GetBerbaStatistikaAsync(berbaId);
                if (statistika == null)
                    return NotFound(new { message = "Berba nije pronađena" });

                return Ok(statistika);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


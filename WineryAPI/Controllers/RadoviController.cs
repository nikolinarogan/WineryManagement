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
    public class RadoviController : ControllerBase
    {
        private readonly IRadoviService _radoviService;

        public RadoviController(IRadoviService radoviService)
        {
            _radoviService = radoviService;
        }

        [HttpGet]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetAllRadovi()
        {
            try
            {
                var radovi = await _radoviService.GetAllRadoviAsync();
                return Ok(radovi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetRadoviById(int id)
        {
            try
            {
                var rad = await _radoviService.GetRadoviByIdAsync(id);
                if (rad == null)
                    return NotFound(new { message = "Rad nije pronađen" });

                return Ok(rad);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> CreateRadovi([FromBody] CreateRadoviDto dto)
        {
            try
            {
                var rad = await _radoviService.CreateRadoviAsync(dto);
                return CreatedAtAction(nameof(GetRadoviById), new { id = rad.Idrad }, rad);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> UpdateRadovi(int id, [FromBody] UpdateRadoviDto dto)
        {
            try
            {
                var success = await _radoviService.UpdateRadoviAsync(id, dto);
                if (!success)
                    return NotFound(new { message = "Rad nije pronađen" });

                return Ok(new { message = "Rad uspješno ažuriran" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> DeleteRadovi(int id)
        {
            try
            {
                var success = await _radoviService.DeleteRadoviAsync(id);
                if (!success)
                    return NotFound(new { message = "Rad nije pronađen" });

                return Ok(new { message = "Rad uspješno obrisan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{radId}/parcele/{parcelaId}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> AddParcelaToRad(int radId, int parcelaId)
        {
            try
            {
                var success = await _radoviService.AddParcelaToRadAsync(radId, parcelaId);
                if (!success)
                    return NotFound(new { message = "Rad ili parcela nisu pronađeni" });

                return Ok(new { message = "Parcela uspješno dodata na rad" });
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

        [HttpDelete("{radId}/parcele/{parcelaId}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> RemoveParcelaFromRad(int radId, int parcelaId)
        {
            try
            {
                var success = await _radoviService.RemoveParcelaFromRadAsync(radId, parcelaId);
                if (!success)
                    return NotFound(new { message = "Parcela nije pronađena na radu" });

                return Ok(new { message = "Parcela uspješno uklonjena sa rada" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{radId}/radnici")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> AddRadnikToRad(int radId, [FromBody] AddRadnikToRadDto dto)
        {
            try
            {
                var success = await _radoviService.AddRadnikToRadAsync(radId, dto);
                if (!success)
                    return NotFound(new { message = "Rad ili radnik nisu pronađeni" });

                return Ok(new { message = "Radnik uspješno dodat na rad" });
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

        [HttpDelete("{radId}/radnici/{radnikId}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> RemoveRadnikFromRad(int radId, int radnikId)
        {
            try
            {
                var success = await _radoviService.RemoveRadnikFromRadAsync(radId, radnikId);
                if (!success)
                    return NotFound(new { message = "Radnik nije pronađen na radu" });

                return Ok(new { message = "Radnik uspješno uklonjen sa rada" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("moji-radovi")]
        [Authorize(Roles = "Radnik")]
        public async Task<IActionResult> GetMojiRadovi()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var radovi = await _radoviService.GetRadoviForRadnikAsync(userId);
                return Ok(radovi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Enolog,Menadzer")]
    public class SeLagerujeController : ControllerBase
    {
        private readonly ISeLagerujeService _seLagerujeService;

        public SeLagerujeController(ISeLagerujeService seLagerujeService)
        {
            _seLagerujeService = seLagerujeService;
        }

        [HttpGet("dostupna-buradi")]
        public async Task<IActionResult> GetDostupnaBurad()
        {
            try
            {
                var buradi = await _seLagerujeService.GetDostupnaBuradAsync();
                return Ok(buradi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSvaLagerovanja()
        {
            try
            {
                var lagerovanja = await _seLagerujeService.GetSvaLagerovanjaAsync();
                return Ok(lagerovanja);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("sirovo-vino/{sirovovinoId}")]
        public async Task<IActionResult> GetLagerovanjaZaSirovoVino(int sirovovinoId)
        {
            try
            {
                var lagerovanja = await _seLagerujeService.GetLagerovanjaZaSirovoVinoAsync(sirovovinoId);
                return Ok(lagerovanja);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Enolog")]
        public async Task<IActionResult> StartLagerovanje([FromBody] CreateLagerovanjeDto dto)
        {
            try
            {
                var lagerovanje = await _seLagerujeService.StartLagerovanjeAsync(dto);
                return Ok(lagerovanje);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpPut("{sirovovinoId}/{bureId}")]
        [Authorize(Roles = "Enolog")]
        public async Task<IActionResult> UpdateLagerovanje(int sirovovinoId, int bureId, [FromBody] UpdateLagerovanjeDto dto)
        {
            try
            {
                await _seLagerujeService.UpdateLagerovanjeAsync(sirovovinoId, bureId, dto);
                return Ok(new { message = "Datum pražnjenja uspješno ažuriran" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
    }
}


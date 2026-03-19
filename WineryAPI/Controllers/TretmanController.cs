using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Enolog")]
    public class TretmanController : ControllerBase
    {
        private readonly ITretmanService _service;

        public TretmanController(ITretmanService service)
        {
            _service = service;
        }

        [HttpGet("ubrane-sirovine")]
        public async Task<IActionResult> GetAllUbraneSirovine()
        {
            try
            {
                var sirovine = await _service.GetAllUbraneSirovineAsync();
                return Ok(sirovine);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ubrane-sirovine/{id}")]
        public async Task<IActionResult> GetUbranaSirovinaById(int id)
        {
            try
            {
                var sirovina = await _service.GetUbranaSirovinaByIdAsync(id);

                if (sirovina == null)
                    return NotFound(new { message = "Ubrana sirovina nije pronađena" });

                return Ok(sirovina);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ubrane-sirovine/{ubranasirovinaId}/tretmani")]
        public async Task<IActionResult> GetTretmaniForUbranaSirovina(int ubranasirovinaId)
        {
            try
            {
                var tretmani = await _service.GetTretmaniForUbranaSirovinaAsync(ubranasirovinaId);
                return Ok(tretmani);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTretmanDetail(int id)
        {
            try
            {
                var tretman = await _service.GetTretmanDetailAsync(id);

                if (tretman == null)
                    return NotFound(new { message = "Tretman nije pronađen" });

                return Ok(tretman);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTretman([FromBody] CreateTretmanDto dto)
        {
            try
            {
                var enologIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(enologIdClaim) || !int.TryParse(enologIdClaim, out int enologId))
                    return Unauthorized(new { message = "Enolog nije autentifikovan" });

                var tretman = await _service.CreateTretmanAsync(dto, enologId);
                return CreatedAtAction(nameof(GetTretmanDetail), new { id = tretman.Idtretmana }, tretman);
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

        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseTretman(int id, [FromBody] CloseTretmanDto dto)
        {
            try
            {
                await _service.CloseTretmanAsync(id, dto);
                return NoContent();
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

        [HttpPost("{tretmanId}/sirovine")]
        public async Task<IActionResult> AddSirovinaToTretman(int tretmanId, [FromBody] AddSirovinaToTretmanDto dto)
        {
            try
            {
                await _service.AddSirovinaToTretmanAsync(tretmanId, dto);
                return NoContent();
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

        [HttpDelete("{tretmanId}/sirovine/{sirovinazatretmanIdsir}")]
        public async Task<IActionResult> RemoveSirovinaFromTretman(int tretmanId, int sirovinazatretmanIdsir)
        {
            try
            {
                await _service.RemoveSirovinaFromTretmanAsync(tretmanId, sirovinazatretmanIdsir);
                return NoContent();
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

        [HttpGet("aktivni")]
        public async Task<IActionResult> GetAktivniTretmani()
        {
            try
            {
                var tretmani = await _service.GetAktivniTretmaniAsync();
                return Ok(tretmani);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("zavrseni")]
        public async Task<IActionResult> GetZavreniTretmani()
        {
            try
            {
                var tretmani = await _service.GetZavreniTretmaniAsync();
                return Ok(tretmani);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


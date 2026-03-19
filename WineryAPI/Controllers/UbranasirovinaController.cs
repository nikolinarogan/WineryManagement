using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Menadzer")]
    public class UbranasirovinaController : ControllerBase
    {
        private readonly IUbranasirovinaService _service;

        public UbranasirovinaController(IUbranasirovinaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrijemi()
        {
            try
            {
                var prijemi = await _service.GetAllPrijemiAsync();
                return Ok(prijemi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrijemById(int id)
        {
            try
            {
                var prijem = await _service.GetPrijemByIdAsync(id);

                if (prijem == null)
                    return NotFound(new { message = "Prijem nije pronađen" });

                return Ok(prijem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("spremni-za-prijem")]
        public async Task<IActionResult> GetRasporedsReadyForPrijem()
        {
            try
            {
                var rasporedi = await _service.GetRasporedsReadyForPrijemAsync();
                return Ok(rasporedi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("raspored/{rasporedId}/detalji")]
        public async Task<IActionResult> GetPrijemDetails(int rasporedId)
        {
            try
            {
                var details = await _service.GetPrijemDetailsAsync(rasporedId);

                if (details == null)
                    return NotFound(new { message = "Raspored nije pronađen" });

                return Ok(details);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePrijem([FromBody] ValidatePrijemRequestDto dto)
        {
            try
            {
                var result = await _service.ValidatePrijemAsync(dto.RasporedId, dto.Kolicina);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrijem([FromBody] CreateUbranasirovinaDto dto)
        {
            try
            {
                var prijem = await _service.CreatePrijemAsync(dto);
                return CreatedAtAction(nameof(GetPrijemById), new { id = prijem.Idubrsir }, prijem);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrijem(int id, [FromBody] UpdateUbranasirovinaDto dto)
        {
            try
            {
                var success = await _service.UpdatePrijemAsync(id, dto);

                if (!success)
                    return NotFound(new { message = "Prijem nije pronađen" });

                return Ok(new { message = "Prijem uspješno ažuriran" });
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrijem(int id)
        {
            try
            {
                var success = await _service.DeletePrijemAsync(id);

                if (!success)
                    return NotFound(new { message = "Prijem nije pronađen" });

                return Ok(new { message = "Prijem uspješno obrisan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // Helper DTO za validaciju
    public class ValidatePrijemRequestDto
    {
        public int RasporedId { get; set; }
        public decimal Kolicina { get; set; }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Enolog")]
    public class SirovinazatretmanController : ControllerBase
    {
        private readonly ISirovinazatretmanService _service;

        public SirovinazatretmanController(ISirovinazatretmanService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSirovine()
        {
            try
            {
                var sirovine = await _service.GetAllSirovineAsync();
                return Ok(sirovine);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSirovinaById(int id)
        {
            try
            {
                var sirovina = await _service.GetSirovinaByIdAsync(id);

                if (sirovina == null)
                    return NotFound(new { message = "Sirovina nije pronađena" });

                return Ok(sirovina);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSirovina([FromBody] CreateSirovinazatretmanDto dto)
        {
            try
            {
                var sirovina = await _service.CreateSirovinaAsync(dto);
                return CreatedAtAction(nameof(GetSirovinaById), new { id = sirovina.Idsir }, sirovina);
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
        public async Task<IActionResult> UpdateSirovina(int id, [FromBody] UpdateSirovinazatretmanDto dto)
        {
            try
            {
                await _service.UpdateSirovinaAsync(id, dto);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSirovina(int id)
        {
            try
            {
                await _service.DeleteSirovinaAsync(id);
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
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Enolog")]
    public class SirovovinoController : ControllerBase
    {
        private readonly ISirovovinoService _service;

        public SirovovinoController(ISirovovinoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSirovina()
        {
            try
            {
                var sirovina = await _service.GetAllSirovovinaAsync();
                return Ok(sirovina);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSirovovinoById(int id)
        {
            try
            {
                var sirovovino = await _service.GetSirovovinoByIdAsync(id);

                if (sirovovino == null)
                    return NotFound(new { message = "Sirovo vino nije pronađeno" });

                return Ok(sirovovino);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSirovovino([FromBody] CreateSirovovinoDto dto)
        {
            try
            {
                var sirovovino = await _service.CreateSirovovinoAsync(dto);
                return CreatedAtAction(nameof(GetSirovovinoById), new { id = sirovovino.Idsirvina }, sirovovino);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSirovovino(int id, [FromBody] UpdateSirovovinoDto dto)
        {
            try
            {
                await _service.UpdateSirovovinoAsync(id, dto);
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
        public async Task<IActionResult> DeleteSirovovino(int id)
        {
            try
            {
                await _service.DeleteSirovovinoAsync(id);
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


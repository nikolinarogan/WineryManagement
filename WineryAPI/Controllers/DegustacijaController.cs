using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DegustacijaController : ControllerBase
    {
        private readonly IDegustacijaService _degustacijaService;

        public DegustacijaController(IDegustacijaService degustacijaService)
        {
            _degustacijaService = degustacijaService;
        }

        [HttpGet]
        [Authorize(Roles = "Somleijer")]
        public async Task<IActionResult> GetAllDegustacije()
        {
            try
            {
                var degustacije = await _degustacijaService.GetAllDegustacijeAsync();
                return Ok(degustacije);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Somleijer")]
        public async Task<IActionResult> GetDegustacijaById(int id)
        {
            try
            {
                var degustacija = await _degustacijaService.GetDegustacijaByIdAsync(id);
                return Ok(degustacija);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Somleijer")]
        public async Task<IActionResult> CreateDegustacija([FromBody] CreateDegustacijaDto dto)
        {
            try
            {
                var degustacija = await _degustacijaService.CreateDegustacijaAsync(dto);
                return CreatedAtAction(nameof(GetDegustacijaById), new { id = degustacija.Iddeg }, degustacija);
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
        [Authorize(Roles = "Somleijer")]
        public async Task<IActionResult> UpdateDegustacija(int id, [FromBody] UpdateDegustacijaDto dto)
        {
            try
            {
                var degustacija = await _degustacijaService.UpdateDegustacijaAsync(id, dto);
                return Ok(degustacija);
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
        [Authorize(Roles = "Somleijer")]
        public async Task<IActionResult> DeleteDegustacija(int id)
        {
            try
            {
                await _degustacijaService.DeleteDegustacijaAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


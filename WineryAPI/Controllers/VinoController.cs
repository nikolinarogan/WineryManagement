using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VinoController : ControllerBase
    {
        private readonly IVinoService _vinoService;

        public VinoController(IVinoService vinoService)
        {
            _vinoService = vinoService;
        }

        [HttpGet]
        [Authorize(Roles = "Enolog,Menadzer,Somleijer")]
        public async Task<IActionResult> GetAllVina()
        {
            try
            {
                var vina = await _vinoService.GetAllVinaAsync();
                return Ok(vina);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Enolog,Menadzer")]
        public async Task<IActionResult> GetVinoById(int id)
        {
            try
            {
                var vino = await _vinoService.GetVinoByIdAsync(id);
                if (vino == null)
                    return NotFound(new { message = "Vino nije pronađeno" });

                return Ok(vino);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Enolog")]
        public async Task<IActionResult> CreateVino([FromBody] CreateVinoDto dto)
        {
            try
            {
                var vino = await _vinoService.CreateVinoAsync(dto);
                return CreatedAtAction(nameof(GetVinoById), new { id = vino.Idvina }, vino);
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
        [Authorize(Roles = "Enolog")]
        public async Task<IActionResult> UpdateVino(int id, [FromBody] UpdateVinoDto dto)
        {
            try
            {
                await _vinoService.UpdateVinoAsync(id, dto);
                return Ok(new { message = "Vino uspješno ažurirano" });
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
        [Authorize(Roles = "Enolog")]
        public async Task<IActionResult> DeleteVino(int id)
        {
            try
            {
                await _vinoService.DeleteVinoAsync(id);
                return Ok(new { message = "Vino uspješno obrisano" });
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


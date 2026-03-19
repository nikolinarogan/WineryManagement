using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MagacinController : ControllerBase
    {
        private readonly IMagacinService _magacinService;

        public MagacinController(IMagacinService magacinService)
        {
            _magacinService = magacinService;
        }

        [HttpGet]
        [Authorize(Roles = "Enolog,Menadzer")]
        public async Task<IActionResult> GetAllMagacini()
        {
            try
            {
                var magacini = await _magacinService.GetAllMagaciniAsync();
                return Ok(magacini);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Enolog,Menadzer")]
        public async Task<IActionResult> GetMagacinById(int id)
        {
            try
            {
                var magacin = await _magacinService.GetMagacinByIdAsync(id);
                if (magacin == null)
                    return NotFound(new { message = "Magacin nije pronađen" });

                return Ok(magacin);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> CreateMagacin([FromBody] CreateMagacinDto dto)
        {
            try
            {
                var magacin = await _magacinService.CreateMagacinAsync(dto);
                return CreatedAtAction(nameof(GetMagacinById), new { id = magacin.Idmag }, magacin);
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
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> UpdateMagacin(int id, [FromBody] UpdateMagacinDto dto)
        {
            try
            {
                await _magacinService.UpdateMagacinAsync(id, dto);
                return Ok(new { message = "Magacin uspješno ažuriran" });
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
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> DeleteMagacin(int id)
        {
            try
            {
                await _magacinService.DeleteMagacinAsync(id);
                return Ok(new { message = "Magacin uspješno obrisan" });
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


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Menadzer")]
    public class BureController : ControllerBase
    {
        private readonly IBureService _bureService;

        public BureController(IBureService bureService)
        {
            _bureService = bureService;
        }

        [HttpGet("podrum/{podrumId}")]
        public async Task<IActionResult> GetBuradiByPodrumId(int podrumId)
        {
            try
            {
                var buradi = await _bureService.GetBuradiByPodrumIdAsync(podrumId);
                return Ok(buradi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBureById(int id)
        {
            try
            {
                var bure = await _bureService.GetBureByIdAsync(id);
                if (bure == null)
                    return NotFound(new { message = "Bure nije pronađeno" });

                return Ok(bure);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBure([FromBody] CreateBureDto dto)
        {
            try
            {
                var bure = await _bureService.CreateBureAsync(dto);
                return CreatedAtAction(nameof(GetBureById), new { id = bure.Idbur }, bure);
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
        public async Task<IActionResult> UpdateBure(int id, [FromBody] UpdateBureDto dto)
        {
            try
            {
                await _bureService.UpdateBureAsync(id, dto);
                return Ok(new { message = "Bure uspješno ažurirano" });
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
        public async Task<IActionResult> DeleteBure(int id)
        {
            try
            {
                await _bureService.DeleteBureAsync(id);
                return Ok(new { message = "Bure uspješno obrisano" });
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


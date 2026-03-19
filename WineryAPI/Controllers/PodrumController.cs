using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Menadzer")]
    public class PodrumController : ControllerBase
    {
        private readonly IPodrumService _podrumService;

        public PodrumController(IPodrumService podrumService)
        {
            _podrumService = podrumService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPodrumi()
        {
            try
            {
                var podrumi = await _podrumService.GetAllPodrumiAsync();
                return Ok(podrumi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPodrumById(int id)
        {
            try
            {
                var podrum = await _podrumService.GetPodrumByIdAsync(id);
                if (podrum == null)
                    return NotFound(new { message = "Podrum nije pronađen" });

                return Ok(podrum);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePodrum([FromBody] CreatePodrumDto dto)
        {
            try
            {
                var podrum = await _podrumService.CreatePodrumAsync(dto);
                return CreatedAtAction(nameof(GetPodrumById), new { id = podrum.Idpod }, podrum);
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
        public async Task<IActionResult> UpdatePodrum(int id, [FromBody] UpdatePodrumDto dto)
        {
            try
            {
                await _podrumService.UpdatePodrumAsync(id, dto);
                return Ok(new { message = "Podrum uspješno ažuriran" });
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
        public async Task<IActionResult> DeletePodrum(int id)
        {
            try
            {
                await _podrumService.DeletePodrumAsync(id);
                return Ok(new { message = "Podrum uspješno obrisan" });
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


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Menadzer")]
    public class SortagrozdjaController : ControllerBase
    {
        private readonly ISortagrozdjaService _sortagrozdjaService;

        public SortagrozdjaController(ISortagrozdjaService sortagrozdjaService)
        {
            _sortagrozdjaService = sortagrozdjaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSorte()
        {
            try
            {
                var sorte = await _sortagrozdjaService.GetAllSorteAsync();
                return Ok(sorte);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSortaById(int id)
        {
            try
            {
                var sorta = await _sortagrozdjaService.GetSortaByIdAsync(id);
                if (sorta == null)
                    return NotFound(new { message = "Sorta nije pronađena" });

                return Ok(sorta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSorta([FromBody] CreateSortagrozdjaDto dto)
        {
            try
            {
                var sorta = await _sortagrozdjaService.CreateSortaAsync(dto);
                return CreatedAtAction(nameof(GetSortaById), new { id = sorta.Idsrt }, sorta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSorta(int id, [FromBody] UpdateSortagrozdjaDto dto)
        {
            try
            {
                var success = await _sortagrozdjaService.UpdateSortaAsync(id, dto);
                if (!success)
                    return NotFound(new { message = "Sorta nije pronađena" });

                return Ok(new { message = "Sorta uspješno ažurirana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSorta(int id)
        {
            try
            {
                var success = await _sortagrozdjaService.DeleteSortaAsync(id);
                if (!success)
                    return NotFound(new { message = "Sorta nije pronađena" });

                return Ok(new { message = "Sorta uspješno obrisana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


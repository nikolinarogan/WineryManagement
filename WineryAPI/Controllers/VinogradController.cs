using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Menadzer")]
    public class VinogradController : ControllerBase
    {
        private readonly IVinogradService _vinogradService;

        public VinogradController(IVinogradService vinogradService)
        {
            _vinogradService = vinogradService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllVinogradi()
        {
            try
            {
                var vinogradi = await _vinogradService.GetAllVinogradiAsync();
                return Ok(vinogradi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("with-parcele")]
        public async Task<IActionResult> GetAllVinogradiWithParcele()
        {
            try
            {
                var vinogradi = await _vinogradService.GetAllVinogradiWithParcelaAsync();
                return Ok(vinogradi);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVinogradById(int id)
        {
            try
            {
                var vinograd = await _vinogradService.GetVinogradByIdAsync(id);
                if (vinograd == null)
                    return NotFound(new { message = "Vinograd nije pronađen" });

                return Ok(vinograd);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateVinograd([FromBody] CreateVinogradDto dto)
        {
            try
            {
                var vinograd = await _vinogradService.CreateVinogradAsync(dto);
                return CreatedAtAction(nameof(GetVinogradById), new { id = vinograd.Idv }, vinograd);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVinograd(int id, [FromBody] UpdateVinogradDto dto)
        {
            try
            {
                var success = await _vinogradService.UpdateVinogradAsync(id, dto);
                if (!success)
                    return NotFound(new { message = "Vinograd nije pronađen" });

                return Ok(new { message = "Vinograd uspješno ažuriran" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVinograd(int id)
        {
            try
            {
                var success = await _vinogradService.DeleteVinogradAsync(id);
                if (!success)
                    return NotFound(new { message = "Vinograd nije pronađen" });

                return Ok(new { message = "Vinograd uspješno obrisan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        [HttpPost("{vinogradId}/parcele")]
        public async Task<IActionResult> AddParcela(int vinogradId, [FromBody] CreateParcelaDto dto)
        {
            try
            {
                var parcela = await _vinogradService.AddParcelaToVinogradAsync(vinogradId, dto);
                return Ok(parcela);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPut("parcele/{parcelaId}")]
        public async Task<IActionResult> UpdateParcela(int parcelaId, [FromBody] UpdateParcelaDto dto)
        {
            try
            {
                var success = await _vinogradService.UpdateParcelaAsync(parcelaId, dto);
                if (!success)
                    return NotFound(new { message = "Parcela nije pronađena" });

                return Ok(new { message = "Parcela uspješno ažurirana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpDelete("parcele/{parcelaId}")]
        public async Task<IActionResult> DeleteParcela(int parcelaId)
        {
            try
            {
                var success = await _vinogradService.DeleteParcelaAsync(parcelaId);
                if (!success)
                    return NotFound(new { message = "Parcela nije pronađena" });

                return Ok(new { message = "Parcela uspješno obrisana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


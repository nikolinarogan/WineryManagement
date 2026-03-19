using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BocaController : ControllerBase
    {
        private readonly IBocaService _bocaService;

        public BocaController(IBocaService bocaService)
        {
            _bocaService = bocaService;
        }

        [HttpPost("punjenje")]
        [Authorize(Roles = "Enolog")]
        public async Task<IActionResult> PunjenjeBoca([FromBody] CreateBocaPunjenjeDto dto)
        {
            try
            {
                var result = await _bocaService.CreateBocePunjenjeAsync(dto);
                return Ok(result);
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

        [HttpGet]
        [Authorize(Roles = "Enolog,Menadzer")]
        public async Task<IActionResult> GetAllBoce()
        {
            try
            {
                var boce = await _bocaService.GetAllBoceAsync();
                return Ok(boce);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("magacin/{magacinId}")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetBoceByMagacin(int magacinId)
        {
            try
            {
                var boce = await _bocaService.GetBoceByMagacinAsync(magacinId);
                return Ok(boce);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("magacin/{magacinId}/broj")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetBrojBocaUMagacinu(int magacinId)
        {
            try
            {
                var brojBoca = await _bocaService.GetBrojBocaUMagacinuAsync(magacinId);
                return Ok(new { brojBoca });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


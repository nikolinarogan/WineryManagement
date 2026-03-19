using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WineryAPI.DTOs;
using WineryAPI.Services;

namespace WineryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);

                if (response == null)
                    return Unauthorized(new { message = "Neispravni kredencijali" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPost("register")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // id menadz koji kreira radnika, iy tokena trenutnog ulogovanog uzmem id
                var menadzerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                registerDto.SuperiorId = menadzerId;

                var response = await _authService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(GetCurrentUser), new { id = response.Idzap }, response);//vratim 201 creayed
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri kreiranju korisnika", error = ex.Message });
            }
        }

        
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var success = await _authService.ChangePasswordAsync(userId, changePasswordDto);

                if (!success)
                    return BadRequest(new { message = "Neispravna stara lozinka" });

                return Ok(new { message = "Lozinka uspješno promijenjena" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

      //trenutni userr
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                idzap = userId,
                email = email,
                name = name,
                kategorija = role
            });
        }

        
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var profile = await _authService.GetProfileAsync(userId);

                if (profile == null)
                    return NotFound(new { message = "Profil nije pronađen" });

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("employees")]
        [Authorize(Roles = "Menadzer")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] string? kategorija = null)
        {
            try
            {
                var employees = await _authService.GetAllEmployeesAsync(kategorija);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


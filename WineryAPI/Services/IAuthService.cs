using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ChangePasswordAsync(int zaposleniId, ChangePasswordDto changePasswordDto);
        Task<ProfileDto?> GetProfileAsync(int zaposleniId);
        Task<List<EmployeeListDto>> GetAllEmployeesAsync(string? kategorija = null);
    }
}


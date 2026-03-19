using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface ISirovovinoService
    {
        Task<List<SirovovinoDto>> GetAllSirovovinaAsync();
        Task<SirovovinoDto?> GetSirovovinoByIdAsync(int id);
        Task<SirovovinoDto> CreateSirovovinoAsync(CreateSirovovinoDto dto);
        Task UpdateSirovovinoAsync(int id, UpdateSirovovinoDto dto);
        Task DeleteSirovovinoAsync(int id);
    }
}

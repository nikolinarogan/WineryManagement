using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IVinoService
    {
        Task<List<VinoDto>> GetAllVinaAsync();
        Task<VinoDetailDto?> GetVinoByIdAsync(int id);
        Task<VinoDto> CreateVinoAsync(CreateVinoDto dto);
        Task UpdateVinoAsync(int id, UpdateVinoDto dto);
        Task DeleteVinoAsync(int id);
    }
}


using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IBureService
    {
        Task<List<BureDto>> GetBuradiByPodrumIdAsync(int podrumId);
        Task<BureDetailDto?> GetBureByIdAsync(int id);
        Task<BureDto> CreateBureAsync(CreateBureDto dto);
        Task UpdateBureAsync(int id, UpdateBureDto dto);
        Task DeleteBureAsync(int id);
    }
}


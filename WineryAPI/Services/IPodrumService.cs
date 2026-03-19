using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IPodrumService
    {
        Task<List<PodrumDto>> GetAllPodrumiAsync();
        Task<PodrumDetailDto?> GetPodrumByIdAsync(int id);
        Task<PodrumDto> CreatePodrumAsync(CreatePodrumDto dto);
        Task UpdatePodrumAsync(int id, UpdatePodrumDto dto);
        Task DeletePodrumAsync(int id);
    }
}


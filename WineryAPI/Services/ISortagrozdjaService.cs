using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface ISortagrozdjaService
    {
        Task<List<SortagrozdjaDto>> GetAllSorteAsync();
        Task<SortagrozdjaDetailDto?> GetSortaByIdAsync(int id);
        Task<SortagrozdjaDto> CreateSortaAsync(CreateSortagrozdjaDto dto);
        Task<bool> UpdateSortaAsync(int id, UpdateSortagrozdjaDto dto);
        Task<bool> DeleteSortaAsync(int id);
    }
}


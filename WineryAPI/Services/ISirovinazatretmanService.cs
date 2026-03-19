using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface ISirovinazatretmanService
    {
        Task<List<SirovinazatretmanDto>> GetAllSirovineAsync();
        Task<SirovinazatretmanDto?> GetSirovinaByIdAsync(int id);
        Task<SirovinazatretmanDto> CreateSirovinaAsync(CreateSirovinazatretmanDto dto);
        Task UpdateSirovinaAsync(int id, UpdateSirovinazatretmanDto dto);
        Task DeleteSirovinaAsync(int id);
    }
}


using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IMagacinService
    {
        Task<List<MagacinDto>> GetAllMagaciniAsync();
        Task<MagacinDetailDto?> GetMagacinByIdAsync(int id);
        Task<MagacinDto> CreateMagacinAsync(CreateMagacinDto dto);
        Task UpdateMagacinAsync(int id, UpdateMagacinDto dto);
        Task DeleteMagacinAsync(int id);
    }
}


using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IDegustacijaService
    {
        Task<List<DegustacijaBasicDto>> GetAllDegustacijeAsync();
        Task<DegustacijaDto> GetDegustacijaByIdAsync(int id);
        Task<DegustacijaDto> CreateDegustacijaAsync(CreateDegustacijaDto dto);
        Task<DegustacijaDto> UpdateDegustacijaAsync(int id, UpdateDegustacijaDto dto);
        Task DeleteDegustacijaAsync(int id);
    }
}


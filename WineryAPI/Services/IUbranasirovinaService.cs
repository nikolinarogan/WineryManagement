using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IUbranasirovinaService
    {
        Task<List<UbranasirovinaDto>> GetAllPrijemiAsync();
        
        Task<UbranasirovinaDto?> GetPrijemByIdAsync(int id);
        
        Task<List<RasporedZaPrijemDto>> GetRasporedsReadyForPrijemAsync();
        
        Task<PrijemDetailsDto?> GetPrijemDetailsAsync(int rasporedId);
        
        Task<PrijemValidationResultDto> ValidatePrijemAsync(int rasporedId, decimal kolicina);
        
        Task<UbranasirovinaDto> CreatePrijemAsync(CreateUbranasirovinaDto dto);
        
        Task<bool> UpdatePrijemAsync(int id, UpdateUbranasirovinaDto dto);
        
        Task<bool> DeletePrijemAsync(int id);
    }
}


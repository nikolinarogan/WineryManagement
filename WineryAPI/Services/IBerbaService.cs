using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IBerbaService
    {
        Task<List<BerbaDto>> GetAllBerbeAsync();
        Task<BerbaDetailDto?> GetBerbaByIdAsync(int id);
        Task<BerbaDto> CreateBerbaAsync(CreateBerbaDto dto);
        Task<bool> UpdateBerbaAsync(int id, UpdateBerbaDto dto);
        Task<bool> DeleteBerbaAsync(int id);
        Task<bool> AddParcelaToBerbaAsync(int berbaId, int parcelaId);
        Task<bool> RemoveParcelaFromBerbaAsync(int berbaId, int parcelaId);
        
        // RasporedBranja methods
        Task<List<RasporedbranjaDto>> GetAllRasporedbranjaAsync(int? berbaId = null);
        Task<RasporedbranjaDetailDto?> GetRasporedbranjaByIdAsync(int id);
        Task<RasporedbranjaDto> CreateRasporedbranjaAsync(CreateRasporedbranjaDto dto);
        Task<bool> UpdateRasporedbranjaAsync(int id, UpdateRasporedbranjaDto dto);
        Task<bool> DeleteRasporedbranjaAsync(int id);
        Task<bool> AddRadnikToRasporedAsync(int rasporedId, AddRadnikToRasporedDto dto);
        Task<bool> RemoveRadnikFromRasporedAsync(int rasporedId, int radnikId);
        Task<List<RasporedbranjaDto>> GetRasporediForRadnikAsync(int radnikId);
        Task<List<RadnikRasporedDto>> GetMojiRasporediDetaljiAsync(int radnikId);
        Task<bool> UpdateMojaKolicinaAsync(int radnikId, int rasporedId, UpdateRadnikKolicinaDto dto);
        Task<BerbaStatistikaDto?> GetBerbaStatistikaAsync(int berbaId);
    }
}


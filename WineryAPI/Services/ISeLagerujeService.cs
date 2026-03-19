using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface ISeLagerujeService
    {
        Task<List<DostupnoBureDto>> GetDostupnaBuradAsync();
        Task<List<LagerovanoVinoDto>> GetLagerovanjaZaSirovoVinoAsync(int sirovovinoId);
        Task<List<LagerovanoVinoDto>> GetSvaLagerovanjaAsync();
        Task<LagerovanoVinoDto> StartLagerovanjeAsync(CreateLagerovanjeDto dto);
        Task UpdateLagerovanjeAsync(int sirovovinoId, int bureId, UpdateLagerovanjeDto dto);
    }
}


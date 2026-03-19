using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IBocaService
    {
        Task<BocePunjenjeResultDto> CreateBocePunjenjeAsync(CreateBocaPunjenjeDto dto);
        Task<List<BocaInventarDto>> GetAllBoceAsync();
        Task<List<BocaDto>> GetBoceByMagacinAsync(int magacinId);
        Task<int> GetBrojBocaUMagacinuAsync(int magacinId);
    }
}


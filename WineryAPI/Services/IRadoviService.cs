using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IRadoviService
    {
        Task<List<RadoviDto>> GetAllRadoviAsync();
        Task<RadoviDetailDto?> GetRadoviByIdAsync(int id);
        Task<RadoviDto> CreateRadoviAsync(CreateRadoviDto dto);
        Task<bool> UpdateRadoviAsync(int id, UpdateRadoviDto dto);
        Task<bool> DeleteRadoviAsync(int id);

        Task<bool> AddParcelaToRadAsync(int radId, int parcelaId);
        Task<bool> RemoveParcelaFromRadAsync(int radId, int parcelaId);

        Task<bool> AddRadnikToRadAsync(int radId, AddRadnikToRadDto dto);
        Task<bool> RemoveRadnikFromRadAsync(int radId, int radnikId);

        Task<List<RadnikRadDto>> GetRadoviForRadnikAsync(int radnikId);
    }
}


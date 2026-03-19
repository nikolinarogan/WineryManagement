using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface IVinogradService
    {
        Task<List<VinogradDto>> GetAllVinogradiAsync();
        Task<List<VinogradDetailDto>> GetAllVinogradiWithParcelaAsync();
        Task<VinogradDetailDto?> GetVinogradByIdAsync(int id);
        Task<VinogradDto> CreateVinogradAsync(CreateVinogradDto dto);
        Task<bool> UpdateVinogradAsync(int id, UpdateVinogradDto dto);
        Task<bool> DeleteVinogradAsync(int id);
        Task<ParcelaDto> AddParcelaToVinogradAsync(int vinogradId, CreateParcelaDto dto);
        Task<bool> UpdateParcelaAsync(int parcelaId, UpdateParcelaDto dto);
        Task<bool> DeleteParcelaAsync(int parcelaId);
    }
}


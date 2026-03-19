using WineryAPI.DTOs;

namespace WineryAPI.Services
{
    public interface ITretmanService
    {
        Task<List<UbranasirovinaZaTretmanDto>> GetAllUbraneSirovineAsync();
        Task<UbranasirovinaZaTretmanDto?> GetUbranaSirovinaByIdAsync(int id);

        Task<List<TretmanDto>> GetTretmaniForUbranaSirovinaAsync(int ubranasirovinaId);
        Task<TretmanDetailDto?> GetTretmanDetailAsync(int id);
        Task<TretmanDto> CreateTretmanAsync(CreateTretmanDto dto, int enologId);
        Task CloseTretmanAsync(int id, CloseTretmanDto dto);
        
        Task AddSirovinaToTretmanAsync(int tretmanId, AddSirovinaToTretmanDto dto);
        Task RemoveSirovinaFromTretmanAsync(int tretmanId, int sirovinazatretmanIdsir);
        
        Task<List<TretmanDto>> GetAktivniTretmaniAsync();
        
        Task<List<TretmanDto>> GetZavreniTretmaniAsync();
    }
}


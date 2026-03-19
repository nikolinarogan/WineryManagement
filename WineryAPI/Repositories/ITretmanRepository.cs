using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface ITretmanRepository
    {
        // Ubranasirovina queries
        Task<List<Ubranasirovina>> GetAllUbraneSirovineAsync();
        Task<Ubranasirovina?> GetUbranaSirovinaByIdAsync(int id);
        Task<Ubranasirovina?> GetUbranaSirovinaWithDetailsAsync(int id);

        // Tretman CRUD
        Task<List<Tretman>> GetTretmaniByUbranaSirovinaAsync(int ubranasirovinaId);
        Task<Tretman?> GetTretmanByIdAsync(int id);
        Task<Tretman?> GetTretmanWithDetailsAsync(int id);
        Task<Tretman?> GetTretmanWithSirovineAsync(int id);
        Task AddTretmanAsync(Tretman tretman);
        Task UpdateTretmanAsync(Tretman tretman);

        // Tretman filter queries
        Task<List<Tretman>> GetAktivniTretmaniAsync();
        Task<List<Tretman>> GetZavreniTretmaniAsync();

        // Enolog queries
        Task<Enolog?> GetEnologByIdAsync(int id);

        // Sirovinazatretman queries
        Task<Sirovinazatretman?> GetSirovinazatretmanByIdAsync(int id);

        // SeDodaje CRUD
        Task AddSeDodajeAsync(SeDodaje seDodaje);
        Task<SeDodaje?> GetSeDodajeAsync(int tretmanId, int sirovinazatretmanId);
        Task DeleteSeDodajeAsync(SeDodaje seDodaje);
    }
}


using WineryAPI.Models;

namespace WineryAPI.Repositories
{
    public interface IBerbaRepository
    {
        // Berba CRUD
        Task<List<Berba>> GetAllBerbeAsync();
        Task<Berba?> GetBerbaByIdAsync(int id);
        Task<Berba?> GetBerbaWithDetailsAsync(int id);
        Task<Berba> AddBerbaAsync(Berba berba);
        Task UpdateBerbaAsync(Berba berba);
        Task DeleteBerbaAsync(Berba berba);

        // Seodrzava (Berba-Parcela relationship)
        Task<Seodrzava?> GetSeodrzavaAsync(int berbaId, int parcelaId);
        Task AddSeodrzavaAsync(Seodrzava seodrzava);
        Task DeleteSeodrzavaAsync(Seodrzava seodrzava);

        // RasporedBranja CRUD
        Task<List<Rasporedbranja>> GetAllRasporedbranjaAsync(int? berbaId = null);
        Task<Rasporedbranja?> GetRasporedbranjaByIdAsync(int id);
        Task<Rasporedbranja?> GetRasporedbranjaWithDetailsAsync(int id);
        Task<Rasporedbranja> AddRasporedbranjaAsync(Rasporedbranja raspored);
        Task UpdateRasporedbranjaAsync(Rasporedbranja raspored);
        Task DeleteRasporedbranjaAsync(Rasporedbranja raspored);

        // JeAngazovan (RasporedBranja-Radnik relationship)
        Task<JeAngazovan?> GetJeAngazovanAsync(int rasporedId, int radnikId);
        Task<List<JeAngazovan>> GetJeAngazovanByRasporedAsync(int rasporedId);
        Task<List<JeAngazovan>> GetJeAngazovanByRadnikAsync(int radnikId);
        Task AddJeAngazovanAsync(JeAngazovan jeAngazovan);
        Task UpdateJeAngazovanAsync(JeAngazovan jeAngazovan);
        Task DeleteJeAngazovanAsync(JeAngazovan jeAngazovan);

        // Helper methods
        Task<Parcela?> GetParcelaByIdAsync(int parcelaId);
        Task<Radnik?> GetRadnikByIdAsync(int radnikId);
        Task<List<Rasporedbranja>> GetRasporedbranjaByBerbaIdAsync(int berbaId);
        
        // Cross-check validation
        Task<List<Rasporedbranja>> GetRasporedbranjaForRadnikInDateRangeAsync(int radnikId, DateOnly dateFrom, DateOnly dateTo);
        
        Task SaveChangesAsync();
    }
}


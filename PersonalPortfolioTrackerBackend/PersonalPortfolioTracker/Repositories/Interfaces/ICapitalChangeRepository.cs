using PersonalPortfolioTracker.Models;

namespace PersonalPortfolioTracker.Repositories.Interfaces
{
    public interface ICapitalChangeRepository
    {
        Task<CapitalChange?> GetCapitalChangeByIdAsync(int id);
        Task<IEnumerable<CapitalChange>> GetCapitalChangesByPositionIdAsync(int positionId);
        Task<IEnumerable<CapitalChange>> GetAllCapitalChangesAsync();
        Task AddCapitalChangeAsync(CapitalChange capitalChange);
        Task UpdateCapitalChangeAsync(CapitalChange capitalChange);
        Task DeleteCapitalChangeAsync(CapitalChange capitalChange);
    }
}
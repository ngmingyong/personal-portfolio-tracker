using PersonalPortfolioTracker.Models;

namespace PersonalPortfolioTracker.Repositories.Interfaces
{
    public interface IDividendRepository
    {
        Task<Dividend?> GetDividendByIdAsync(int id);
        Task<IEnumerable<Dividend>> GetDividendsByPositionIdAsync(int positionId);
        Task<IEnumerable<Dividend>> GetAllDividendsAsync();
        Task AddDividendAsync(Dividend dividend);
        Task UpdateDividendAsync(Dividend dividend);
        Task DeleteDividendAsync(Dividend dividend);
    }
}
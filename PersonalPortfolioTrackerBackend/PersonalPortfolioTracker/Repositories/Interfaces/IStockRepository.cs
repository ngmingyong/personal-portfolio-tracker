using PersonalPortfolioTracker.Models;

namespace PersonalPortfolioTracker.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task<Stock?> GetStockByCodeAsync(string stockCode);
        Task<IEnumerable<Stock>> GetAllStocksAsync();
        Task AddStockAsync(Stock stock);
        Task UpdateStockAsync(Stock stock);
        Task UpdateStocksAsync(IEnumerable<Stock> stocks);
        Task DeleteStockAsync(Stock stock);
    }
}
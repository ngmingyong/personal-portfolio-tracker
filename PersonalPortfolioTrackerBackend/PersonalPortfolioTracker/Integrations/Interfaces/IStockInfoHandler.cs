using PersonalPortfolioTracker.DTOs.Response;

namespace PersonalPortfolioTracker.Integrations.Interfaces
{
    public interface IStockInfoHandler
    {
        Task<StockInfoDto[]?> GetStockInfoAsync(string stockCode, string exchange);
    }
}
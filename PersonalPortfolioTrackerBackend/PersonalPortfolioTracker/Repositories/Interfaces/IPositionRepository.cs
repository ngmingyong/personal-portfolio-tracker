using PersonalPortfolioTracker.Models;

namespace PersonalPortfolioTracker.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task<Position?> GetPositionByIdAsync(int id);
        Task<IEnumerable<Position>> GetPositionsByStockIdAsync(int stockId);
        Task<IEnumerable<Position>> GetAllPositionsAsync();
        Task AddPositionAsync(Position position);
        Task UpdatePositionAsync(Position position);
        Task DeletePositionAsync(Position position);
    }
}
using Microsoft.EntityFrameworkCore;
using PersonalPortfolioTracker.Data;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;

namespace PersonalPortfolioTracker.Repositories.Implementations
{
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _context;

        public PositionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Position?> GetPositionByIdAsync(int id)
        {
            return await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Position>> GetPositionsByStockIdAsync(int stockId)
        {
            return await _context.Positions.Where(p => p.StockId == stockId).ToListAsync();
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
        {
            return await _context.Positions.ToListAsync();
        }

        public async Task AddPositionAsync(Position position)
        {
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePositionAsync(Position position)
        {
            _context.Positions.Update(position);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePositionAsync(Position position)
        {
            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
        }
    }
}
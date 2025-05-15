using Microsoft.EntityFrameworkCore;
using PersonalPortfolioTracker.Data;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;

namespace PersonalPortfolioTracker.Repositories.Implementations
{
    public class DividendRepository : IDividendRepository
    {
        private readonly AppDbContext _context;

        public DividendRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dividend?> GetDividendByIdAsync(int id)
        {
            return await _context.Dividends.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Dividend>> GetDividendsByPositionIdAsync(int positionId)
        {
            return await _context.Dividends.Where(d => d.PositionId == positionId).ToListAsync();
        }

        public async Task<IEnumerable<Dividend>> GetAllDividendsAsync()
        {
            return await _context.Dividends.ToListAsync();
        }

        public async Task AddDividendAsync(Dividend dividend)
        {
            _context.Dividends.Add(dividend);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDividendAsync(Dividend dividend)
        {
            _context.Dividends.Update(dividend);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDividendAsync(Dividend dividend)
        {
            _context.Dividends.Remove(dividend);
            await _context.SaveChangesAsync();
        }
    }
}
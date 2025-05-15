using Microsoft.EntityFrameworkCore;
using PersonalPortfolioTracker.Data;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;

namespace PersonalPortfolioTracker.Repositories.Implementations
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;

        public StockRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockByCodeAsync(string stockCode)
        {
            return await _context.Stocks.Where(s => EF.Functions.Like(s.Code, stockCode)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await _context.Stocks.ToListAsync();
        }

        public async Task AddStockAsync(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(Stock stock)
        {
            _context.Stocks.Update(stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStocksAsync(IEnumerable<Stock> stocks)
        {
            _context.Stocks.UpdateRange(stocks);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStockAsync(Stock stock)
        {
            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
        }
    }
}
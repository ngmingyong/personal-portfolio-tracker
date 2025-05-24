using Microsoft.EntityFrameworkCore;
using PersonalPortfolioTracker.Data;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;

namespace PersonalPortfolioTracker.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByPositionIdAsync(int positionId)
        {
            return await _context.Transactions.Where(t => t.PositionId == positionId).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
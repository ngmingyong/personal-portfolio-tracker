using Microsoft.EntityFrameworkCore;
using PersonalPortfolioTracker.Data;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;

namespace PersonalPortfolioTracker.Repositories.Implementations
{
    public class CapitalChangeRepository : ICapitalChangeRepository
    {
        private readonly AppDbContext _context;

        public CapitalChangeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CapitalChange?> GetCapitalChangeByIdAsync(int id)
        {
            return await _context.CapitalChanges.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<CapitalChange>> GetCapitalChangesByPositionIdAsync(int positionId)
        {
            return await _context.CapitalChanges.Where(cc => cc.PositionId == positionId).ToListAsync();
        }

        public async Task<IEnumerable<CapitalChange>> GetAllCapitalChangesAsync()
        {
            return await _context.CapitalChanges.ToListAsync();
        }

        public async Task AddCapitalChangeAsync(CapitalChange capitalChange)
        {
            _context.CapitalChanges.Add(capitalChange);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCapitalChangeAsync(CapitalChange capitalChange)
        {
            _context.CapitalChanges.Update(capitalChange);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCapitalChangeAsync(CapitalChange capitalChange)
        {
            _context.CapitalChanges.Remove(capitalChange);
            await _context.SaveChangesAsync();
        }
    }
}
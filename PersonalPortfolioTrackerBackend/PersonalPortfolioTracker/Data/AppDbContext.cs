using Microsoft.EntityFrameworkCore;
using PersonalPortfolioTracker.Models;

namespace PersonalPortfolioTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Dividend> Dividends { get; set; }
        public DbSet<CapitalChange> CapitalChanges { get; set; }

        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<CapitalChangeType> CapitalChangeTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique Stock Code
            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Code)
                .IsUnique();

            // Stock - Position (one-to-many relationship)
            modelBuilder.Entity<Position>()
                .HasOne(p => p.Stock)
                .WithMany(s => s.Positions)
                .HasForeignKey(p => p.StockId);

            // Position - Transaction (one-to-many relationship)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Position)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PositionId);

            // Transaction Type - Transaction (one-to-many relationship)
            modelBuilder.Entity<Transaction>()
                .HasOne<TransactionType>()
                .WithMany()
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Position - Dividend (one-to-many relationship)
            modelBuilder.Entity<Dividend>()
                .HasOne(d=> d.Position)
                .WithMany(p => p.Dividends)
                .HasForeignKey(d => d.PositionId);

            // Position - Capital Change (one-to-many relationship)
            modelBuilder.Entity<CapitalChange>()
                .HasOne(cc => cc.Position)
                .WithMany(p => p.CapitalChanges)
                .HasForeignKey(cc => cc.PositionId);

            // Capital Change Type - Capital Change (one-to-many relationship)
            modelBuilder.Entity<CapitalChange>()
                .HasOne<CapitalChangeType>()
                .WithMany()
                .HasForeignKey(t => t.CapitalChangeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Initial data for Transaction Type
            modelBuilder.Entity<TransactionType>()
                .HasData(
                    Enum.GetValues(typeof(Enums.TransactionType))
                    .Cast<Enums.TransactionType>()
                    .Select(type => new TransactionType
                    {
                        Id = (int)type,
                        Name = type.ToString()
                    })
                );

            // Initial data for Capital Change Type
            modelBuilder.Entity<CapitalChangeType>()
                .HasData(
                    Enum.GetValues(typeof(Enums.CapitalChangeType))
                    .Cast<Enums.CapitalChangeType>()
                    .Select(type => new CapitalChangeType
                    {
                        Id = (int)type,
                        Name = type.ToString()
                    })
                );
        }
    }
}
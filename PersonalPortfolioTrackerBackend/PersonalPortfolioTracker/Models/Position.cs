using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalPortfolioTracker.Models
{
    [Table("position")]
    public class Position
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("position_opened_date", TypeName = "DATE")]
        public DateOnly PositionOpenedDate { get; set; }

        [Column("position_closed_date", TypeName = "DATE")]
        public DateOnly PositionClosedDate { get; set; }

        [Column("is_position_closed")]
        public bool IsPositionClosed { get; set; }

        [Column("no_of_shares_held")]
        public int NoOfSharesHeld { get; set; }

        [Column("total_purchase_cost")]
        public decimal TotalPurchaseCost { get; set; }

        [Column("total_dividends_received")]
        public decimal TotalDividendsReceived { get; set; }

        [Column("total_net_sales_proceeds")]
        public decimal TotalNetSalesProceeds { get; set; }

        [Column("stock_id")]
        public int StockId { get; set; }

        public required Stock Stock { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }

        public ICollection<Dividend>? Dividends { get; set; }

        public ICollection<CapitalChange>? CapitalChanges { get; set; }
    }
}
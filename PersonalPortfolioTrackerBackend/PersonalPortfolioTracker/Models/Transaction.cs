using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalPortfolioTracker.Models
{
    [Table("transaction")]
    public class Transaction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("date", TypeName = "DATE")]
        public DateOnly Date { get; set; }

        [Column("no_of_shares_transacted")]
        public int NoOfSharesTransacted { get; set; }

        [Column("transacted_price_per_share")]
        public decimal TransactedPricePerShare { get; set; }

        [Column("total_transaction_related_expenses")]
        public decimal TotalTransactionRelatedExpenses { get; set; }

        [Column("transaction_type_id")]
        public int TransactionTypeId { get; set; }

        [Column("position_id")]
        public int PositionId { get; set; }

        public required Position Position { get; set; }
    }
}
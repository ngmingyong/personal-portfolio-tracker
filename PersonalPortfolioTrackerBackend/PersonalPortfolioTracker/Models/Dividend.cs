using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalPortfolioTracker.Models
{
    [Table("dividend")]
    public class Dividend
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("entitlement_date", TypeName = "DATE")]
        public DateOnly EntitlementDate { get; set; }

        [Column("no_of_shares_eligible")]
        public int NoOfSharesEligible { get; set; }

        [Column("dividend_per_share")]
        public decimal DividendPerShare { get; set; }

        [Column("is_subject_to_withholding_tax")]
        public bool IsSubjectToWithholdingTax { get; set; }

        [Column("withholding_tax")]
        public decimal WithholdingTax { get; set; }

        [Column("position_id")]
        public int PositionId { get; set; }

        public required Position Position { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalPortfolioTracker.Models
{
    [Table("capital_change")]
    public class CapitalChange
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("entitlement_date", TypeName = "DATE")]
        public DateOnly EntitlementDate { get; set; }

        [Column("change_in_no_of_shares")]
        public int ChangeInNoOfShares { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("capital_change_type_id")]
        public int CapitalChangeTypeId { get; set; }

        [Column("position_id")]
        public int PositionId { get; set; }

        public required Position Position { get; set; }
    }
}
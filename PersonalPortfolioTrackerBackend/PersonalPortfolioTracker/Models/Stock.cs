using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalPortfolioTracker.Models
{
    [Table("stock")]
    public class Stock
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("last_updated", TypeName = "DATE")]
        public DateOnly LastUpdated { get; set; }

        public ICollection<Position>? Positions { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Request
{
    public class AddTransactionRequestDto
    {
        [JsonPropertyName("stockCode")]
        public string? StockCode { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("noOfSharesTransacted")]
        public int NoOfSharesTransacted { get; set; }

        [JsonPropertyName("transactedPricePerShare")]
        public decimal TransactedPricePerShare { get; set; }

        [JsonPropertyName("totalTransactionRelatedExpenses")]
        public decimal TotalTransactionRelatedExpenses { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Response
{
    public class TransactionResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("noOfSharesTransacted")]
        public int NoOfSharesTransacted { get; set; }

        [JsonPropertyName("transactedPricePerShare")]
        public decimal TransactedPricePerShare { get; set; }

        [JsonPropertyName("transactionValueBeforeExpenses")]
        public decimal TransactionValueBeforeExpenses { get; set; }

        [JsonPropertyName("totalTransactionRelatedExpenses")]
        public decimal TotalTransactionRelatedExpenses { get; set; }

        [JsonPropertyName("purchaseCostOrNetSalesProceeds")]
        public decimal PurchaseCostOrNetSalesProceeds { get; set; }
    }
}
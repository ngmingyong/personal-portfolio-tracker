using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Response
{
    public class PositionResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("stockCode")]
        public string? StockCode { get; set; }

        [JsonPropertyName("stockName")]
        public string? StockName { get; set; }

        [JsonPropertyName("stockPrice")]
        public decimal StockPrice { get; set; }

        [JsonPropertyName("stockPriceLastUpdated")]
        public string? StockPriceLastUpdated { get; set; }

        [JsonPropertyName("positionOpenedDate")]
        public string? PositionOpenedDate { get; set; }

        [JsonPropertyName("positionClosedDate")]
        public string? PositionClosedDate { get; set; }

        [JsonPropertyName("isPositionClosed")]
        public bool IsPositionClosed { get; set; }

        [JsonPropertyName("noOfSharesHeld")]
        public int NoOfSharesHeld { get; set; }

        [JsonPropertyName("totalPurchaseCost")]
        public decimal TotalPurchaseCost { get; set; }

        [JsonPropertyName("totalDividendsReceived")]
        public decimal TotalDividendsReceived { get; set; }

        [JsonPropertyName("totalNetSalesProceeds")]
        public decimal TotalNetSalesProceeds { get; set; }

        [JsonPropertyName("unrealizedValueOfSharesHeldBeforeFinalSalesExpenses")]
        public decimal UnrealizedValueOfSharesHeldBeforeFinalSalesExpenses { get; set; }

        [JsonPropertyName("hypotheticalTotalReturnBeforeFinalSalesExpenses")]
        public decimal HypotheticalTotalReturnBeforeFinalSalesExpenses { get; set; }

        [JsonPropertyName("finalizedTotalReturn")]
        public decimal FinalizedTotalReturn { get; set; }

        [JsonPropertyName("transactions")]
        public IEnumerable<TransactionResponseDto>? Transactions { get; set; }

        [JsonPropertyName("dividends")]
        public IEnumerable<DividendResponseDto>? Dividends { get; set; }

        [JsonPropertyName("capitalChanges")]
        public IEnumerable<CapitalChangeResponseDto>? CapitalChanges { get; set; }
    }
}
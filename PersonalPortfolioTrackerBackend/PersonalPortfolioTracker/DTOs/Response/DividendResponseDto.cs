using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Response
{
    public class DividendResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("entitlementDate")]
        public string? EntitlementDate { get; set; }

        [JsonPropertyName("noOfSharesEligible")]
        public int NoOfSharesEligible { get; set; }

        [JsonPropertyName("dividendPerShare")]
        public decimal DividendPerShare { get; set; }

        [JsonPropertyName("amountBeforeWithholdingTax")]
        public decimal AmountBeforeWithholdingTax { get; set; }

        [JsonPropertyName("isSubjectToWithholdingTax")]
        public bool IsSubjectToWithholdingTax { get; set; }

        [JsonPropertyName("withholdingTax")]
        public decimal WithholdingTax { get; set; }

        [JsonPropertyName("amountReceived")]
        public decimal AmountReceived { get; set; }
    }
}
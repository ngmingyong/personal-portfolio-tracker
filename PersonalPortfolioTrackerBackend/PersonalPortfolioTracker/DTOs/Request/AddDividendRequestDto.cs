using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Request
{
    public class AddDividendRequestDto
    {
        [JsonPropertyName("positionId")]
        public int PositionId { get; set; }

        [JsonPropertyName("entitlementDate")]
        public string? EntitlementDate { get; set; }

        [JsonPropertyName("noOfSharesEligible")]
        public int NoOfSharesEligible { get; set; }

        [JsonPropertyName("dividendPerShare")]
        public decimal DividendPerShare { get; set; }

        [JsonPropertyName("isSubjectToWithholdingTax")]
        public bool IsSubjectToWithholdingTax { get; set; }

        [JsonPropertyName("withholdingTax")]
        public decimal WithholdingTax { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Response
{
    public class CapitalChangeResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("entitlementDate")]
        public string? EntitlementDate { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("changeInNoOfShares")]
        public int ChangeInNoOfShares { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }
    }
}
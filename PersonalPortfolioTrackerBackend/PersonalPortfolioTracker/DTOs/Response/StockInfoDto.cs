using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Response
{
    public class StockInfoDto
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("exchange")]
        public string? Exchange { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("isin")]
        public string? ISIN { get; set; }

        [JsonPropertyName("previousClose")]
        public decimal PreviousClose { get; set; }

        [JsonPropertyName("previousCloseDate")]
        public string? PreviousCloseDate { get; set; }
    }
}
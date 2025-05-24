using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Response
{
    public class OperationResponseDto
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
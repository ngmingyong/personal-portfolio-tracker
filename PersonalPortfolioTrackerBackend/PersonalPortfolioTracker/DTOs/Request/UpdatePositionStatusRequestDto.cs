using System.Text.Json.Serialization;

namespace PersonalPortfolioTracker.DTOs.Request
{
    public class UpdatePositionStatusRequestDto
    {
        [JsonPropertyName("isPositionClosed")]
        public bool IsPositionClosed { get; set; }
    }
}
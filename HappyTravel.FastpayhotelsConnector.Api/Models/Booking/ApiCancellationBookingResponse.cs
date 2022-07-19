using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiCancellationBookingResponse : ApiMessage
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("cancellationPenalty")]
    public ApiCancellationPenalty CancellationPenalty { get; set; }
}

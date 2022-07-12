using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingInfoRs
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("bookingInfo")]
    public ApiBookingInfo BookingInfo { get; set; }
}

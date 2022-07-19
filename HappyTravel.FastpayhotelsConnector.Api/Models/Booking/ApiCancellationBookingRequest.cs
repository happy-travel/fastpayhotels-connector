using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiCancellationBookingRequest
{
    [JsonPropertyName("bookingCode")]
    public string BookingCode { get; set; }

    [JsonPropertyName("customerCode")]
    public string CustomerCode { get; set; }
}

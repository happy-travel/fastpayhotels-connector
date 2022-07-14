using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingDetailsResponse : ApiMessage
{
    [JsonPropertyName("booking")]
    public ApiBookingRs Booking { get; set; }
}

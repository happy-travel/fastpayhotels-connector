using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingResponse
{
    [JsonPropertyName("result")]
    public ApiBookingInfoRs Result { get; set; }
}

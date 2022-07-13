using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingRequest : ApiMessage
{
    [JsonPropertyName("agencyCode")]
    public string AgencyCode { get; set; }

    [JsonPropertyName("customer")]
    public ApiCustomer Customer { get; set; }

    [JsonPropertyName("rooms")]
    public List<ApiBookingRqRoom> Rooms { get; set; }
}

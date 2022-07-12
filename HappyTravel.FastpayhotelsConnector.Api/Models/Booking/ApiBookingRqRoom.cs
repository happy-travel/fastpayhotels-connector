using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingRqRoom
{
    [JsonPropertyName("paxes")]
    public List<ApiPax> Paxes { get; set; }

    [JsonPropertyName("reservationToken")]
    public string ReservationToken { get; set; }
}

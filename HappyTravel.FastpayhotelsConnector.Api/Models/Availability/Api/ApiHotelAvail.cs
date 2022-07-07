using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiHotelAvail
{
    [JsonPropertyName("hotelInfo")]
    public ApiHotelInfo HotelInfo { get; set; }

    [JsonPropertyName("availRoomRates")]
    public List<ApiAvailRoomRate> AvailRoomRates { get; set; }
}

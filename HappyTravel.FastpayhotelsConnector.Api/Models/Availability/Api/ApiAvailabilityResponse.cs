using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiAvailabilityResponse
{
    [JsonPropertyName("hotelAvails")]
    public List<ApiHotelAvail> HotelAvails { get; set; }
}

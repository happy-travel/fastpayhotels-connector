using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiAvailabilityRequest : ApiMessage
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("checkIn")]
    public string CheckIn { get; set; }

    [JsonPropertyName("checkOut")]
    public string CheckOut { get; set; }

    [JsonPropertyName("occupancies")]
    public List<ApiOccupancy> Occupancies { get; set; }

    [JsonPropertyName("hotelCodes")]
    public List<string> HotelCodes { get; set; }

    [JsonPropertyName("parameters")]
    public ApiAvailSearchParameters Parameters { get; set; }
}

using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models;

public class ApiHotelInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }
}

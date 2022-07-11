using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiPreBookRequest
{
    [JsonPropertyName("rooms")]
    public List<ApiPreBookRqRoom> Rooms { get; set; }
}

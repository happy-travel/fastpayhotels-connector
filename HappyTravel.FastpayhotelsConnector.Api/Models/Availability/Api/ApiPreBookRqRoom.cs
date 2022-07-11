using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiPreBookRqRoom
{
    [JsonPropertyName("availToken")]
    public string AvailToken { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}

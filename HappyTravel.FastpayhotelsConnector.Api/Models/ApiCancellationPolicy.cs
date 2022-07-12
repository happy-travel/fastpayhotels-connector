using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models;

public class ApiCancellationPolicy
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("cancellable")]
    public bool Cancellable { get; set; }
}

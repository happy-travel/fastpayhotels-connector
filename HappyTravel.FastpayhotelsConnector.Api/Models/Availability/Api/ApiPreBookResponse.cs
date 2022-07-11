using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiPreBookResponse
{
    [JsonPropertyName("result")]
    public ApiPreBookRsResult Result { get; set; }
}

using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Common.Models;

public class TokenErrorMessage
{
    [JsonPropertyName("error")]
    public string Error { get; set; }

    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; }
}

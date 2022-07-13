using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models;

public class ApiMessage
{
    [JsonPropertyName("messageID")]
    public string MessageId { get; set; }
}

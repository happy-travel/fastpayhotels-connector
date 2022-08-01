using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Common.Models;

/// <summary>
/// Describes the error that occurred while getting the token
/// </summary>
public class TokenErrorMessage
{
    /// <summary>
    /// Error
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; }

    /// <summary>
    /// Error description
    /// </summary>
    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; }
}

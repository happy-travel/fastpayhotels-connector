using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Common.Models;

/// <summary>
/// Token data
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// Token
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Token type
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    /// <summary>
    /// Token Expires in
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

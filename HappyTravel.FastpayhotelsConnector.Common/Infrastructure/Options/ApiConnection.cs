namespace HappyTravel.FastpayhotelsConnector.Common.Infrastructure.Options;

/// <summary>
/// Describes the settings required to connect to the supplier's API
/// </summary>
public class ApiConnection
{
    /// <summary>
    /// Availability requests endpoint
    /// </summary>
    public string AvailabilityEndPoint { get; set; }
    /// <summary>
    /// Booking engine access endpoint
    /// </summary>
    public string BookingEndPoint { get; set; }
    /// <summary>
    /// Portfolio and catalogue endpoint
    /// </summary>
    public string CatalogueEndPoint { get; set; }
    /// <summary>
    /// Client credential client Id
    /// </summary>
    public string ClientId { get; set; }
    /// <summary>
    /// client credential client secret
    /// </summary>
    public string ClientSecret { get; set; }
    /// <summary>
    /// User credentials username
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// User credentials password 
    /// </summary>
    public string Password { get; set; }
}

using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiAvailSearchParameters
{
    [JsonPropertyName("countryOfResidence")]
    public string CountryOfResidence { get; set; }

    [JsonPropertyName("nationality")]
    public string Nationality { get; set; }
}

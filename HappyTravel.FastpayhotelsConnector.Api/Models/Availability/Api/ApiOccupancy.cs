using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiOccupancy
{
    [JsonPropertyName("adults")]
    public int Adults { get; set; }

    [JsonPropertyName("children")]
    public int Children { get; set; }

    [JsonPropertyName("childrenAges")]
    public List<int> ChildrenAges { get; set; }
}

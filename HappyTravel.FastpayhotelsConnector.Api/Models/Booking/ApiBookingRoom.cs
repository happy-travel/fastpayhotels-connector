using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingRoom
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("adults")]
    public int Adults { get; set; }

    [JsonPropertyName("children")]
    public int Children { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("dailyPrices")]
    public List<decimal> DailyPrices { get; set; }

    [JsonPropertyName("paxes")]
    public List<ApiPax> Paxes { get; set; }
}
using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

public class ApiAvailRoomRate
{
    [JsonPropertyName("reservationToken")]
    public string ReservationToken { get; set; }

    [JsonPropertyName("inventory")]
    public int Inventory { get; set; }

    [JsonPropertyName("roomCode")]
    public string RoomCode { get; set; }

    [JsonPropertyName("roomName")]
    public string RoomName { get; set; }

    [JsonPropertyName("occupancy")]
    public ApiOccupancy Occupancy { get; set; }

    [JsonPropertyName("ratePlanCode")]
    public string RatePlanCode { get; set; }

    [JsonPropertyName("ratePlanName")]
    public string RatePlanName { get; set; }

    [JsonPropertyName("mealPlanCode")]
    public string MealPlanCode { get; set; }

    [JsonPropertyName("mealPlanName")]
    public string MealPlanName { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("publicPrice")]
    public decimal PublicPrice { get; set; }

    [JsonPropertyName("priceBinding")]
    public bool PriceBinding { get; set; }

    [JsonPropertyName("commission")]
    public decimal Commission { get; set; }

    [JsonPropertyName("pricePerDay")]
    public List<decimal> PricePerDay { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("promo")]
    public bool Promo { get; set; }

    [JsonPropertyName("cancellationPolicy")]
    public ApiCancellationPolicy CancellationPolicy { get; set; }

    [JsonPropertyName("specialNotes")]
    public string SpecialNotes { get; set; }
}

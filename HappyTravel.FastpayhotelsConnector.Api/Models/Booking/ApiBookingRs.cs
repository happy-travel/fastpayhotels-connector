using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingRs
{
    [JsonPropertyName("hotelCode")]
    public string HotelCode { get; set; }

    [JsonPropertyName("bookingCode")]
    public string BookingCode { get; set; }

    [JsonPropertyName("customerCode")]
    public string CustomerCode { get; set; }

    [JsonPropertyName("propertyDirectReference")]
    public string PropertyDirectReference { get; set; }

    [JsonPropertyName("bookingStatus")]
    public string BookingStatus { get; set; }

    [JsonPropertyName("checkIn")]
    public string CheckIn { get; set; }

    [JsonPropertyName("checkOut")]
    public string CheckOut { get; set; }

    [JsonPropertyName("endCustomer")]
    public ApiCustomer EndCustomer { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; }

    [JsonPropertyName("comments")]
    public List<string> Comments { get; set; }

    [JsonPropertyName("cancellationPolicy")]
    public ApiCancellationPolicy CancellationPolicy { get; set; }

    [JsonPropertyName("cancellationPenalty")]
    public ApiCancellationPenalty CancellationPenalty { get; set; }

    [JsonPropertyName("specialNotes")]
    public string SpecialNotes { get; set; }

    [JsonPropertyName("bookingrooms")]
    public List<ApiBookingRoom> BookingRooms { get; set; }
}

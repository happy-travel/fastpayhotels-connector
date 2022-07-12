using System.Text.Json.Serialization;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingInfo
{
    [JsonPropertyName("bookingCode")]
    public string BookingCode { get; set; }

    [JsonPropertyName("agencyCode")]
    public string AgencyCode { get; set; }

    [JsonPropertyName("checkIn")]
    public string CheckIn { get; set; }

    [JsonPropertyName("checkOut")]
    public string CheckOut { get; set; }

    [JsonPropertyName("hotelInfo")]
    public ApiHotelInfo HotelInfo { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("commission")]
    public decimal Commission { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("comments")]
    public string Comments { get; set; }

    [JsonPropertyName("cancellationPolicy")]
    public ApiCancellationPolicy CancellationPolicy { get; set; }

    [JsonPropertyName("specialNotes")]
    public string SpecialNotes { get; set; }

    [JsonPropertyName("rooms")]
    public List<ApiBookingRoomRate> Rooms { get; set; }
}

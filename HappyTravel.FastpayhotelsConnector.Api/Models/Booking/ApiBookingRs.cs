namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingRs
{
    public string HotelCode { get; set; }
    public string BookingCode { get; set; }
    public string CustomerCode { get; set; }
    public string PropertyDirectReference { get; set; }
    public string BookingStatus { get; set; }
    public string CheckIn { get; set; }
    public string CheckOut { get; set; }
    public ApiCustomer EndCustomer { get; set; }
    public decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; }
    public List<string> Comments { get; set; }
    public ApiCancellationPolicy CancellationPolicy { get; set; }
    public ApiCancellationPenalty CancellationPenalty { get; set; }
    public string SpecialNotes { get; set; }
    public List<ApiBookingRoom> BookingRooms { get; set; }
}

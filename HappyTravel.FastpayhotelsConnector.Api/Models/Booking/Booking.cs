namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class Booking
{
    public string HotelCode { get; set; }
    public string BookingCode { get; set; }
    public string CustomerCode { get; set; }
    public string PropertyDirectReference { get; set; }
    public string BookingStatus { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public EndCustomer EndCustomer { get; set; }
    public double TotalPrice { get; set; }
    public string CurrencyCode { get; set; }
    public List<string> Comments { get; set; }
    public CancellationPolicy CancellationPolicy { get; set; }
    public CancellationPenalty CancellationPenalty { get; set; }
    public string SpecialNotes { get; set; }
    public List<BookingRoom> Bookingrooms { get; set; }
}

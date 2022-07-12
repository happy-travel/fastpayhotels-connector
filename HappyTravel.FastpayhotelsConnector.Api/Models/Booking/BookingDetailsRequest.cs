namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class BookingDetailsRequest
{
    public BookingDetailsRequest(string bookingCode)
    {
        BookingCode = bookingCode;
    }

    public string BookingCode { get; set; }
}

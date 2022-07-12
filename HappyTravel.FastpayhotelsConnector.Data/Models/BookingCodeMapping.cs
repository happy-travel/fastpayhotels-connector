namespace HappyTravel.FastpayhotelsConnector.Data.Models;

public class BookingCodeMapping
{
    public string ReferenceCode { get; set; }
    public string BookingCode { get; set; }
    public List<BookingRoom> BookingRooms { get; set; }
}

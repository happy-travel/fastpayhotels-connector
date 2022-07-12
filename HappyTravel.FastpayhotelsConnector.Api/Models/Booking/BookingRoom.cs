namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class BookingRoom
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public double TotalPrice { get; set; }
    public List<string> DailyPrices { get; set; }
    public List<Pax> Paxes { get; set; }
}
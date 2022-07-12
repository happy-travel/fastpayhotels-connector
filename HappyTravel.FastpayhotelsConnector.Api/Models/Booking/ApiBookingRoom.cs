namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class ApiBookingRoom
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal TotalPrice { get; set; }
    public List<decimal> DailyPrices { get; set; }
    public List<ApiPax> Paxes { get; set; }
}
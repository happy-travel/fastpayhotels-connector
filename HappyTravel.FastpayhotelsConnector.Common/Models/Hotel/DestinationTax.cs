namespace HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;

public class DestinationTax
{
    public string Description { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
    public string GuestScope { get; set; }
    public string DestinationScope { get; set; }
    public bool OnlyForAdults { get; set; }
    public int AgeOver { get; set; }
}
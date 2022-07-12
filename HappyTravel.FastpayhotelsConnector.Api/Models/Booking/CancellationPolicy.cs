namespace HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

public class CancellationPolicy
{
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Cancellable { get; set; }
}
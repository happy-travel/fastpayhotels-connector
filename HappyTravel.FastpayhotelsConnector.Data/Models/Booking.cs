using HappyTravel.EdoContracts.Accommodations.Internals;

namespace HappyTravel.FastpayhotelsConnector.Data.Models;

public class Booking
{
    public string ReferenceCode { get; set; }
    public string BookingCode { get; set; }
    public List<SlimRoomOccupation> Rooms { get; set; }
}
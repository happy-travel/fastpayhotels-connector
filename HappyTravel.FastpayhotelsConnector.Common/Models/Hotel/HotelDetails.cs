using HappyTravel.FastpayhotelsConnector.Common.Models.Catalogue;

namespace HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;

public class HotelDetails
{
    public string Name { get; set; }
    public string ChainName { get; set; }
    public string Phone { get; set; }
    public int TotalRooms { get; set; }
    public RootType HotelType { get; set; }
    public RootType HotelCategory { get; set; }
    public TuristicAreaType Zone { get; set; }
    public List<object> Tags { get; set; }
    public List<object> HostSegments { get; set; }
    public Location Location { get; set; }
    public List<Image> Images { get; set; }
    public List<RootType> MealPlans { get; set; }
    public List<Service> Services { get; set; }
    public List<Description> Descriptions { get; set; }
    public string TimeZone { get; set; }
    public bool AdultsOnly { get; set; }
    public string CheckinHour { get; set; }
    public string CheckoutHour { get; set; }
    public List<Room> Rooms { get; set; }
    public List<DestinationTax> DestinationTaxes { get; set; }
}

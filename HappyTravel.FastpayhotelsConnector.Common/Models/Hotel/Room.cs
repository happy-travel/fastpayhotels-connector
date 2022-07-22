namespace HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;

public class Room : RootType
{
    public List<Description> Descriptions { get; set; }
    public bool AdultsOnly { get; set; }
    public string OccupancyDistribution { get; set; }
    public List<RootType> Beds { get; set; }
    public List<Service> Services { get; set; }
    public List<Image> Images { get; set; }    
}
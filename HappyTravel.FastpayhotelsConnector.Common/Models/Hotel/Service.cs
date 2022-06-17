using HappyTravel.FastpayhotelsConnector.Common.Models.Catalogue;

namespace HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;

public class Service : ServiceType
{
    public bool IsFree { get; set; }
    public RootType Group { get; set; }
}
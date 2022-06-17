using HappyTravel.FastpayhotelsConnector.Common.Models.Catalogue;

namespace HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;

public class Image
{
    public RootType Type { get; set; }
    public string Name { get; set; }
    public List<Caption> Captions { get; set; }
    public bool Featured { get; set; }
    public string Url { get; set; }
}
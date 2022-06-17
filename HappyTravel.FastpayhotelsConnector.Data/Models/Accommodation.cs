using NetTopologySuite.Geometries;

namespace HappyTravel.FastpayhotelsConnector.Data.Models;

public class Accommodation
{
    public string Code { get; set; }
    public string Country { get; set; }
    public string? Locality { get; set; }
    public Point Coordinates { get; set; } = new Point(0, 0);
    public string Name { get; set; }
    public DateTimeOffset Modified { get; set; }
    public TimeSpan? Timezone { get; set; }
}

using System.Globalization;

namespace HappyTravel.FastpayhotelsConnector.Updater.Models;

public class HotelListRequest
{
    public HotelListRequest(DateTimeOffset fromLastUpdateDate)
    {
        FromLastUpdateDate = fromLastUpdateDate.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz", CultureInfo.InvariantCulture);
    }

    public string FromLastUpdateDate { get; set; }
}

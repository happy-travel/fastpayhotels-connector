namespace HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;

public class HotelDetailsRequest
{
    public HotelDetailsRequest(string code)
    {
        Code = code;
        Languages = Constants.Languages.ToList();
    }

    public string Code { get; set; }
    public List<string> Languages { get; set; }
}

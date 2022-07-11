using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;
using HappyTravel.FastpayhotelsConnector.Common.Models;

namespace HappyTravel.FastpayhotelsConnector.Api.Services;

public class BookingTokenAuthHeaderHandler : TokenAuthHeaderHandler
{
    public BookingTokenAuthHeaderHandler(TokenProvider tokenProvider) : base(tokenProvider, FastpayhotelsHttpClients.BookingHttpClient)
    {
    }
}

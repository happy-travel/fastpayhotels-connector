using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;
using HappyTravel.FastpayhotelsConnector.Common.Models;

namespace HappyTravel.FastpayhotelsConnector.Api.Services;

public class AvailabilityTokenAuthHeaderHandler : TokenAuthHeaderHandler
{
    public AvailabilityTokenAuthHeaderHandler(TokenProvider tokenProvider) : base(tokenProvider, FastpayhotelsHttpClients.AvailabilityHttpClient)
    {
    }
}

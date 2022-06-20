using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;
using HappyTravel.FastpayhotelsConnector.Common.Models;

namespace HappyTravel.FastpayhotelsConnector.Updater;

public class CatalogueTokenAuthHeaderHandler : TokenAuthHeaderHandler
{
    public CatalogueTokenAuthHeaderHandler(TokenProvider tokenProvider) : base(tokenProvider, FastpayhotelsHttpClients.CatalogueHttpClient)
    {
    }
}

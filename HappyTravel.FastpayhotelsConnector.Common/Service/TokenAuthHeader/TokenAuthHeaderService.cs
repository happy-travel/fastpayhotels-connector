using CSharpFunctionalExtensions;

namespace HappyTravel.FastpayhotelsConnector.Common.Service.TokenAuthHeader;

public class TokenAuthHeaderService
{
    public TokenAuthHeaderService(TokenAuthHeaderClient tokenAuthHeaderClient,
        TokenAuthHeaderStorage tokenAuthHeaderStorage)
    {
        _tokenAuthHeaderClient = tokenAuthHeaderClient;
        _tokenAuthHeaderStorage = tokenAuthHeaderStorage;
    }


    /// <summary>
    /// Getting a token from the cache. If there is no token in the cache, the token is requested from the supplier and stored in the cache.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<string>> GetOrRequestToken(CancellationToken cancellationToken)
    {
        var token = await _tokenAuthHeaderStorage.Get();

        if (token is not null)
            return token;

        return await _tokenAuthHeaderClient.GetAgencyRolToken(cancellationToken)
            .Tap((authToken) => _tokenAuthHeaderStorage.Set(authToken));
    }


    private readonly TokenAuthHeaderClient _tokenAuthHeaderClient;
    private readonly TokenAuthHeaderStorage _tokenAuthHeaderStorage;
}

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


    public async Task<Result<string>> GetOrSetToken(CancellationToken cancellationToken)
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

using FloxDc.CacheFlow;
using FloxDc.CacheFlow.Extensions;

namespace HappyTravel.FastpayhotelsConnector.Common.Service.TokenAuthHeader;

public class TokenAuthHeaderStorage
{
    public TokenAuthHeaderStorage(IDoubleFlow flow)
    {
        _flow = flow;
    }


    public async Task<string?> Get()
    {
       var token = await _flow.GetAsync<string?>(BuildKey(), AuthTokenLifeTime);

        return token;
    }


    public Task Set(string token)
        => _flow.SetAsync(BuildKey(), token, AuthTokenLifeTime);


    private string BuildKey()
        => _flow.BuildKey(nameof(TokenAuthHeaderStorage), TokenKey);


    private const string TokenKey = "TokenAuthHeader";

    private static TimeSpan AuthTokenLifeTime => Constants.AuthTokenLifeTime;

    private readonly IDoubleFlow _flow;
}

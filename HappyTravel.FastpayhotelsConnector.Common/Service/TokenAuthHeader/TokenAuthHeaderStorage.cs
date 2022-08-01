using FloxDc.CacheFlow;
using FloxDc.CacheFlow.Extensions;

namespace HappyTravel.FastpayhotelsConnector.Common.Service.TokenAuthHeader;

public class TokenAuthHeaderStorage
{
    public TokenAuthHeaderStorage(IDoubleFlow flow)
    {
        _flow = flow;
    }


    /// <summary>
    /// Retrieves a token from the cache
    /// </summary>
    /// <returns></returns>
    public async Task<string?> Get()
    {
       var token = await _flow.GetAsync<string?>(BuildKey(), AuthTokenLifeTime);

        return token;
    }


    /// <summary>
    /// Saving the token in the cache
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task Set(string token)
        => _flow.SetAsync(BuildKey(), token, AuthTokenLifeTime);


    private string BuildKey()
        => _flow.BuildKey(nameof(TokenAuthHeaderStorage), TokenKey);


    private const string TokenKey = "TokenAuthHeader";

    private static TimeSpan AuthTokenLifeTime => Constants.AuthTokenLifeTimeDays;

    private readonly IDoubleFlow _flow;
}

using HappyTravel.FastpayhotelsConnector.Common.Models;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;

namespace HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;

public abstract class TokenAuthHeaderHandler : DelegatingHandler
{
    public TokenAuthHeaderHandler(TokenProvider tokenProvider, FastpayhotelsHttpClients fastpayhotelsHttpClient)
    {
        _tokenProvider = tokenProvider;
        _fastpayhotelsHttpClient = fastpayhotelsHttpClient;
        _policy = Policy
            .HandleResult<HttpResponseMessage>(x => x.StatusCode is System.Net.HttpStatusCode.Unauthorized)
            .RetryAsync((_, onRetry) => tokenProvider.GetAccessTokenAsync(fastpayhotelsHttpClient));
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => await _policy.ExecuteAsync(async () =>
        {
            var token = await _tokenProvider.GetAccessTokenAsync(_fastpayhotelsHttpClient);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        });


    private readonly AsyncRetryPolicy<HttpResponseMessage> _policy;
    private readonly TokenProvider _tokenProvider;
    private readonly FastpayhotelsHttpClients _fastpayhotelsHttpClient;
}

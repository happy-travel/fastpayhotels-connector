using Polly;
using Polly.Retry;
using System.Net.Http.Headers;

namespace HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;

public class TokenAuthHeaderHandler : DelegatingHandler
{
    public TokenAuthHeaderHandler(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
        _policy = Policy
            .HandleResult<HttpResponseMessage>(x => x.StatusCode is System.Net.HttpStatusCode.Unauthorized)
            .RetryAsync((_, onRetry) => tokenProvider.GetAccessTokenAsync());
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => await _policy.ExecuteAsync(async () =>
        {
            var token = await _tokenProvider.GetAccessTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        });


    private readonly AsyncRetryPolicy<HttpResponseMessage> _policy;
    private readonly TokenProvider _tokenProvider;
}

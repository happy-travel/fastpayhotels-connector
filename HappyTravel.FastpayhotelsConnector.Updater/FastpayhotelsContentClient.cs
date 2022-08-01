using CSharpFunctionalExtensions;
using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Service.TokenAuthHeader;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HappyTravel.FastpayhotelsConnector.Updater;

public class FastpayhotelsContentClient
{
    public FastpayhotelsContentClient(IHttpClientFactory httpClientFactory, 
        FastpayhotelsSerializer serializer, 
        TokenAuthHeaderService tokenAuthHeaderService)
    {
        _httpClientFactory = httpClientFactory;
        _serializer = serializer;
        _tokenAuthHeaderService = tokenAuthHeaderService;
    }    


    private Task<TResponse> Post<TRequest, TResponse>(Uri url, TRequest requestContent, CancellationToken cancellationToken)
        => Send<TResponse>(new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(_serializer.Serialize(requestContent), Encoding.UTF8, "application/json"),
            }, cancellationToken);


    private async Task<TResponse> Send<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (_, isFailure, token, error) = await _tokenAuthHeaderService.GetOrRequestToken(cancellationToken);
        if (isFailure)
            throw new Exception(error);

        var client = _httpClientFactory.CreateClient(HttpClientNames.FastpayhotelsClient);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await client.SendAsync(request, cancellationToken);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var streamReader = new StreamReader(stream);
        using var jsonTextReader = new JsonTextReader(streamReader);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);

        return _serializer.Deserialize<TResponse>(jsonTextReader);
    }


    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FastpayhotelsSerializer _serializer;
    private readonly TokenAuthHeaderService _tokenAuthHeaderService;
}

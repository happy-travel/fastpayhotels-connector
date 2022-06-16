using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Models;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace HappyTravel.FastpayhotelsConnector.Updater;

public class FastpayhotelsContentClient
{
    public FastpayhotelsContentClient(IHttpClientFactory httpClientFactory, FastpayhotelsSerializer serializer, IOptions<ApiConnection> apiConnection)
    {
        _httpClientFactory = httpClientFactory;
        _serializer = serializer;
        _apiConnection = apiConnection.Value;
    }    


    private Task<TResponse> Post<TRequest, TResponse>(Uri url, TRequest requestContent, CancellationToken cancellationToken)
            => Send<TResponse>(new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(_serializer.Serialize(requestContent), Encoding.UTF8, "application/json"),
            }, cancellationToken);


    private async Task<TResponse> Send<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.FastpayhotelsClient);
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
    private readonly ApiConnection _apiConnection;
}

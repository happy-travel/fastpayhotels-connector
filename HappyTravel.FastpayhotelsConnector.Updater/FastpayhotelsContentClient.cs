using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Models;
using HappyTravel.FastpayhotelsConnector.Common.Models.Catalogue;
using HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using HappyTravel.FastpayhotelsConnector.Updater.Models;
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


    public Task<CatalogueResponse> GetCatalogue(CatalogueRequest request, CancellationToken cancellationToken)
    {
        var url = $"{_apiConnection.CatalogueUrl}/api/hotel/catalogue";

        return Post<CatalogueRequest, CatalogueResponse>(new Uri(url, UriKind.Absolute), request, cancellationToken);
    }


    public Task<HotelListResponse> GetHotelList(HotelListRequest request, CancellationToken cancellationToken)
    {
        var url = $"{_apiConnection.CatalogueUrl}/api/hotel/list";

        return Post<HotelListRequest, HotelListResponse>(new Uri(url, UriKind.Absolute), request, cancellationToken);
    }


    public Task<HotelDetailsResponse> GetHotelDetails(HotelDetailsRequest request, CancellationToken cancellationToken)
    {
        var url = $"{_apiConnection.CatalogueUrl}/api/hotel/details";

        return Post<HotelDetailsRequest, HotelDetailsResponse>(new Uri(url, UriKind.Absolute), request, cancellationToken);
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

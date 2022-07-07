using CSharpFunctionalExtensions;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Logging;
using HappyTravel.FastpayhotelsConnector.Api.Models;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;
using System.Text;
using System.Text.Json;

namespace HappyTravel.FastpayhotelsConnector.Api;

public class FastpayhotelsShoppingClient
{
    public FastpayhotelsShoppingClient(IHttpClientFactory httpClientFactory,
            ILogger<FastpayhotelsShoppingClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }


    public async Task<Result<ApiAvailabilityResponse>> GetAvailability(ApiAvailabilityRequest request,  CancellationToken cancellationToken)
    {
        const string endpointUrl = "api/booking/availability";

        return await Post<ApiAvailabilityRequest, ApiAvailabilityResponse>(HttpClientNames.FastpayhotelsAvailabilityClient, new Uri(endpointUrl, UriKind.Relative), request, cancellationToken);
    }


    private Task<Result<TResponse>> Post<TRequest, TResponse>(string httpClientName, Uri url, TRequest requestContent, CancellationToken cancellationToken)
        => Send<TResponse>(httpClientName, 
        new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json")
        }, 
        cancellationToken);


    private async Task<Result<TResponse>> Send<TResponse>(string httpClientName, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(httpClientName);
        using var response = await client.SendAsync(request, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<TResponse>(content);

        string errorMessage = "SupplierStatusCode: {0}, Error: `{1}`";
        try
        {
            var responseError = JsonSerializer.Deserialize<ErrorMessage>(content)?.Message;

            errorMessage = string.Format(errorMessage,
                (int)response.StatusCode,
                !string.IsNullOrWhiteSpace(responseError)
                ? responseError
                : "no message");
        }
        catch (JsonException ex)
        {
            _logger.LogApiResponseDeserializationException($"Cannot deserialize response with error: `{ex.Message}`. Response: {content}, SupplierStatusCode: {(int)response.StatusCode}");
            return Result.Failure<TResponse>("Server error");
        }

        return Result.Failure<TResponse>(errorMessage);
    }


    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FastpayhotelsShoppingClient> _logger;
}

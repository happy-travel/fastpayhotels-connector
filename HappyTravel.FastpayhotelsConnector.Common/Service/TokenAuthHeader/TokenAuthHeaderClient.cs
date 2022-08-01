using CSharpFunctionalExtensions;
using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.Options;
using HappyTravel.FastpayhotelsConnector.Common.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HappyTravel.FastpayhotelsConnector.Common.Service.TokenAuthHeader;

public class TokenAuthHeaderClient
{
    public TokenAuthHeaderClient(IHttpClientFactory httpClientFactory, IOptions<ApiConnection> apiConnection)
    {
        _httpClientFactory = httpClientFactory;
        _apiConnection = apiConnection.Value;
    }


    /// <summary>
    /// Returns the token required to authorize the suppliers's API.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<string>> GetAgencyRolToken(CancellationToken cancellationToken)
    {
        const string endPoint = "/security/token";

        var (isSuccess, _, tokenResponse, error) = await Post<TokenResponse>(new Uri(endPoint, UriKind.Relative), GetСontent(), cancellationToken);

        return isSuccess
            ? tokenResponse.AccessToken
            : Result.Failure<string>(error);


        FormUrlEncodedContent GetСontent()
            => new(new List<KeyValuePair<string, string>>()
            {
                new(Constants.HeaderGrandtype, HeaderGrandtypeValue),
                new(Constants.HeaderClientId, _apiConnection.ClientId),
                new(Constants.HeaderClientSecret, _apiConnection.ClientSecret),
                new(Constants.HeaderVersion, Constants.ApiVersion),
                new(Constants.HeaderUser, _apiConnection.UserName),
                new(Constants.HeaderPassword, _apiConnection.Password),
            });
    }


    private Task<Result<TResponse>> Post<TResponse>(Uri uri, HttpContent content, CancellationToken cancellationToken)
        => Send<TResponse>(new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = content
        }, cancellationToken);


    private async Task<Result<TResponse>> Send<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(Constants.FastpayhotelsTokenClient);
        using var response = await client.SendAsync(request, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode) 
            return JsonSerializer.Deserialize<TResponse>(content);

        try
        {
            var responseError = JsonSerializer.Deserialize<TokenErrorMessage>(content);

            return Result.Failure<TResponse>($"Failed Authentication, StatusCode: {response.StatusCode}, Error: `{responseError.Error}`, Description: `{responseError.ErrorDescription}`");
        }
        catch (JsonException ex)
        {
            return Result.Failure<TResponse>("Server error");
        }
    }


    private const string HeaderGrandtypeValue = "password";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiConnection _apiConnection;
}

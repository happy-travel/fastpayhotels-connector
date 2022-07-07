using HappyTravel.FastpayhotelsConnector.Common.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;

public class TokenProvider
{
    public TokenProvider(IOptions<ApiConnection> apiConnection)
    {
        _apiConnection = apiConnection.Value;        
    }


    public async Task<string> GetAccessTokenAsync(FastpayhotelsHttpClients fastpayhotelsHttpClient)
    {
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri(GetBaseAddres())
        };

        var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>(Constants.HeaderGrandtype, HeaderGrandtypeValue),
            new KeyValuePair<string, string>(Constants.HeaderClientId, _apiConnection.ClientId),
            new KeyValuePair<string, string>(Constants.HeaderClientSecret, _apiConnection.ClientSecret),
            new KeyValuePair<string, string>(Constants.HeaderVersion, Constants.ApiVersion),
            new KeyValuePair<string, string>(Constants.HeaderUser, _apiConnection.UserName),
            new KeyValuePair<string, string>(Constants.HeaderPassword, _apiConnection.Password)
        });      

        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(TokenUrl, UriKind.Relative))
        { 
            Content = content
        };

        var response = await client.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, jsonOptions);
            return tokenResponse.AccessToken;
        }

        throw new Exception($"Failed Authentication, StatusCode: {response.StatusCode}, {response.RequestMessage}");


        string GetBaseAddres()
            => fastpayhotelsHttpClient switch
            {
                FastpayhotelsHttpClients.AvailabilityHttpClient => _apiConnection.AvailabilityEndPoint,
                FastpayhotelsHttpClients.BookingHttpClient => _apiConnection.BookingEndPoint,
                FastpayhotelsHttpClients.CatalogueHttpClient => _apiConnection.CatalogueEndPoint,
                _ => _apiConnection.AvailabilityEndPoint,
            };
    }


    private const string HeaderGrandtypeValue = "password";
    private const string TokenUrl = "/security/token";

    private readonly ApiConnection _apiConnection;    
}

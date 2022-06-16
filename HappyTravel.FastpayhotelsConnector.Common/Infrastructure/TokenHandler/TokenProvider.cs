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


    public async Task<string> GetAccessTokenAsync()
    {
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri(_apiConnection.CatalogueUrl)
        };

        var contentValues = new List<KeyValuePair<string, string>>();
        contentValues.Add(new KeyValuePair<string, string>(Constants.HeaderGrandtype, HeaderGrandtypeValue));
        contentValues.Add(new KeyValuePair<string, string>(Constants.HeaderClientId, _apiConnection.ClientId));
        contentValues.Add(new KeyValuePair<string, string>(Constants.HeaderClientSecret, _apiConnection.ClientSecret));
        contentValues.Add(new KeyValuePair<string, string>(Constants.HeaderVersion, Constants.ApiVersion));
        contentValues.Add(new KeyValuePair<string, string>(Constants.HeaderUser, _apiConnection.User));
        contentValues.Add(new KeyValuePair<string, string>(Constants.HeaderPassword, _apiConnection.Password));


        var content = new FormUrlEncodedContent(contentValues);        

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
    }


    private const string HeaderGrandtypeValue = "password";
    private const string TokenUrl = "/security/token";

    private readonly ApiConnection _apiConnection;    
}

using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.Environment;
using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;
using HappyTravel.FastpayhotelsConnector.Common.Models;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using HappyTravel.VaultClient;

namespace HappyTravel.FastpayhotelsConnector.Updater;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        using var vaultClient = new VaultClient.VaultClient(new VaultOptions
        {
            BaseUrl = new Uri(EnvironmentVariableHelper.Get("Vault:Endpoint", Configuration)),
            Engine = Configuration["Vault:Engine"],
            Role = Configuration["Vault:Role"]
        });

        vaultClient.Login(EnvironmentVariableHelper.Get("Vault:Token", Configuration), LoginMethods.Token)?.GetAwaiter().GetResult();
        var apiConnectionOptions = vaultClient.Get("fastpayhotels-connector/api-connection").GetAwaiter().GetResult();

        services.AddHttpClient(HttpClientNames.FastpayhotelsClient, client =>
        {
            client.BaseAddress = new Uri(apiConnectionOptions["catalogueUrl"]);            
        })
        .AddHttpMessageHandler<CatalogueTokenAuthHeaderHandler>();

        services.AddTransient<FastpayhotelsContext>();
        services.AddTransient<FastpayhotelsContentClient>();
        services.AddTransient<FastpayhotelsSerializer>();        
        services.AddTransient<CatalogueTokenAuthHeaderHandler>();
        services.AddTransient<TokenProvider>();        

        ConfigureWorkers(services);

        services.AddHealthChecks();

        services.Configure<ApiConnection>(o =>
        {
            o.ClientId = apiConnectionOptions["clientId"];
            o.ClientSecret = apiConnectionOptions["clientSecret"];
            o.User = apiConnectionOptions["user"];
            o.Password = apiConnectionOptions["password"];
            o.CatalogueEndPoint = apiConnectionOptions["catalogueUrl"];
        });
    }


    private void ConfigureWorkers(IServiceCollection services)
    {
        
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHealthChecks("/health");
    }


    public IConfiguration Configuration { get; }
}

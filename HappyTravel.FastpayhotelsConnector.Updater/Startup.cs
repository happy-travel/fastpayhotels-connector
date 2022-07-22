using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.Environment;
using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;
using HappyTravel.FastpayhotelsConnector.Common.Models;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using HappyTravel.FastpayhotelsConnector.Updater.Service;
using HappyTravel.FastpayhotelsConnector.Updater.Settings;
using HappyTravel.FastpayhotelsConnector.Updater.Workers;
using HappyTravel.VaultClient;
using Microsoft.EntityFrameworkCore;

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
        
        services.AddTransient<HotelLoader>();  
        services.AddTransient<HotelUpdater>();
        services.AddTransient<AccommodationUpdater>();

        services.AddHostedService<StaticDataUpdateHostedService>();
        services.AddTransient<DateTimeProvider>();

        ConfigureDatabaseOptions(services, vaultClient);
        ConfigureWorkers(services);

        services.AddHealthChecks();

        services.Configure<ApiConnection>(o =>
        {
            o.ClientId = apiConnectionOptions["clientId"];
            o.ClientSecret = apiConnectionOptions["clientSecret"];
            o.UserName = apiConnectionOptions["userName"];
            o.Password = apiConnectionOptions["password"];
            o.CatalogueEndPoint = apiConnectionOptions["catalogueEndPoint"];
        });

        services.Configure<RawDataUpdateOptions>(Configuration.GetSection("Workers:RawUpdateOptions"));
    }


    private void ConfigureWorkers(IServiceCollection services)
    {
        var workersToRun = Configuration.GetSection("Workers:WorkersToRun").Value;
        if (string.IsNullOrWhiteSpace(workersToRun))
        {
            services.AddTransient<IUpdateWorker, HotelLoader>();
            services.AddTransient<IUpdateWorker, AccommodationUpdater>();
        }
        else
        {
            foreach (var workerName in workersToRun.Split(';').Select(s => s.Trim()))
            {                
                if (workerName == nameof(HotelLoader))
                    services.AddTransient<IUpdateWorker, HotelLoader>();
                if (workerName == nameof(AccommodationUpdater))
                    services.AddTransient<IUpdateWorker, AccommodationUpdater>();
            }
        }
    }


    private void ConfigureDatabaseOptions(IServiceCollection services, VaultClient.VaultClient vaultClient)
    {
        var databaseOptions = vaultClient.Get(Configuration["Database:Options"]).GetAwaiter().GetResult();

        services.AddDbContext<FastpayhotelsContext>(options =>
        {
            var host = databaseOptions["host"];
            var port = databaseOptions["port"];
            var password = databaseOptions["password"];
            var userId = databaseOptions["userId"];

            var connectionString = Configuration["Database:ConnectionString"];
            options.UseNpgsql(string.Format(connectionString, host, port, userId, password), builder =>
            {
                builder.UseNetTopologySuite();
                builder.EnableRetryOnFailure();
            });
            options.UseInternalServiceProvider(null);
            options.EnableSensitiveDataLogging(false);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }, ServiceLifetime.Singleton, ServiceLifetime.Singleton);
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHealthChecks("/health");
    }


    public IConfiguration Configuration { get; }
}

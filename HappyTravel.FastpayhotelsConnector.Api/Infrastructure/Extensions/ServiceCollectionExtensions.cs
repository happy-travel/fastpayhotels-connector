using HappyTravel.FastpayhotelsConnector.Api.Services;
using HappyTravel.FastpayhotelsConnector.Common.Infrastructure.TokenHandler;
using HappyTravel.FastpayhotelsConnector.Common.Models;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.HttpRequestAuditLogger.Extensions;
using HappyTravel.HttpRequestLogger;
using HappyTravel.VaultClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using System.Reflection;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        => services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "HappyTravel.com Fastpayhotels API", Version = "v1.0" });

            var xmlCommentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsFilePath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFileName);

            c.IncludeXmlComments(xmlCommentsFilePath);
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            });
        })
        .AddSwaggerGenNewtonsoftSupport();


    public static IServiceCollection ConfigureDatabaseOptions(this IServiceCollection services, IVaultClient vaultClient, IConfiguration configuration)
    {
        var databaseOptions = vaultClient.Get(configuration["Database:Options"]).GetAwaiter().GetResult();

        return services.AddDbContext<FastpayhotelsContext>(options =>
        {
            var host = databaseOptions["host"];
            var port = databaseOptions["port"];
            var password = databaseOptions["password"];
            var userId = databaseOptions["userId"];

            var connectionString = configuration["Database:ConnectionString"];
            options.UseNpgsql(string.Format(connectionString, host, port, userId, password), builder =>
            {
                builder.UseNetTopologySuite();
                builder.EnableRetryOnFailure();
            });
            options.UseInternalServiceProvider(null);
            options.EnableSensitiveDataLogging(false);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);
    }


    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services, IVaultClient vaultClient, IConfiguration configuration)
    {
        var apiConnectionOptions = vaultClient.Get($"{Connector.Name}/api-connection").GetAwaiter().GetResult();
        var fukuokaOptions = vaultClient.Get(configuration["Fukuoka:Options"]).GetAwaiter().GetResult();

        ConfigureApiConnectionOptions();
        ConfigureAvailabilityHttpClient();
        ConfigureBookingHttpClient();

        return services.AddTransient<FastpayhotelsShoppingClient>()
            .AddTransient<TokenProvider>()
            .AddTransient<AvailabilityTokenAuthHeaderHandler>()
            .AddTransient<BookingTokenAuthHeaderHandler>();


        void ConfigureApiConnectionOptions()
            => services.Configure<ApiConnection>(o =>
            {
                o.AvailabilityEndPoint = apiConnectionOptions["availabilityEndPoint"];
                o.BookingEndPoint = apiConnectionOptions["bookingEndPoint"];
                o.ClientId = apiConnectionOptions["clientId"];
                o.ClientSecret = apiConnectionOptions["clientSecret"];
                o.UserName = apiConnectionOptions["userName"];
                o.Password = apiConnectionOptions["password"];
            });


        void ConfigureAvailabilityHttpClient()
        {
            services.AddHttpClient(HttpClientNames.FastpayhotelsAvailabilityClient, client =>
            {
                client.BaseAddress = new Uri(apiConnectionOptions["availabilityEndPoint"]);
            })
            .AddHttpMessageHandler<AvailabilityTokenAuthHeaderHandler>()
            .AddHttpClientRequestLogging(configuration: configuration)
            .UseHttpClientMetrics()
            .AddHttpRequestAudit(options =>
            {
                options.Endpoint = fukuokaOptions["endpoint"];
            });
        }


        void ConfigureBookingHttpClient()
        {
            services.AddHttpClient(HttpClientNames.FastpayhotelsBookingClient, client =>
            {
                client.BaseAddress = new Uri(apiConnectionOptions["bookingEndPoint"]);
            })
            .AddHttpMessageHandler<BookingTokenAuthHeaderHandler>()
            .AddHttpClientRequestLogging(configuration: configuration)
            .UseHttpClientMetrics()
            .AddHttpRequestAudit(options =>
            {
                options.Endpoint = fukuokaOptions["endpoint"];
            });
        }
    }
}

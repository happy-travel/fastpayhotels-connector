using HappyTravel.BaseConnector.Api.Infrastructure.Environment;
using HappyTravel.BaseConnector.Api.Infrastructure.Extensions;
using HappyTravel.BaseConnector.Api.Services.Accommodations;
using HappyTravel.BaseConnector.Api.Services.Availabilities.AccommodationAvailabilities;
using HappyTravel.BaseConnector.Api.Services.Availabilities.Cancellations;
using HappyTravel.BaseConnector.Api.Services.Availabilities.RoomContractSetAvailabilities;
using HappyTravel.BaseConnector.Api.Services.Availabilities.WideAvailabilities;
using HappyTravel.BaseConnector.Api.Services.Bookings;
using HappyTravel.BaseConnector.Api.Services.Locations;
using HappyTravel.ErrorHandling.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Services.Accommodations;
using HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities;
using HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities.AccommodationAvailabilities;
using HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities.Cancellations;
using HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities.RoomContractSetAvailabilities;
using HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities.WideAvailabilities;
using HappyTravel.FastpayhotelsConnector.Api.Services.Bookings;
using HappyTravel.FastpayhotelsConnector.Api.Services.Caching;
using HappyTravel.FastpayhotelsConnector.Api.Services.Locations;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.HttpRequestLogger;
using HappyTravel.VaultClient;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class ServicesConfigurationExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        using var vaultClient = new VaultClient.VaultClient(new VaultOptions
        {
            BaseUrl = new Uri(EnvironmentVariableHelper.Get("Vault:Endpoint", builder.Configuration) ??
                                throw new Exception("Could not find vault endpoint environment variable")),
            Engine = builder.Configuration["Vault:Engine"],
            Role = builder.Configuration["Vault:Role"]
        });

        vaultClient.Login(EnvironmentVariableHelper.Get("Vault:Token", builder.Configuration), LoginMethods.Token)?.GetAwaiter().GetResult();

        builder.Services.AddBaseConnectorServices(builder.Configuration, builder.Environment, vaultClient, Connector.Name);

        builder.Services.AddTransient<HttpRequestLoggingHandler>();

        builder.Services.AddTransient<IAccommodationService, AccommodationService>()
          .AddTransient<IAccommodationAvailabilityService, AccommodationAvailabilityService>()
          .AddTransient<IDeadlineService, DeadlineService>()
          .AddTransient<IRoomContractSetAvailabilityService, RoomContractSetAvailabilityService>()
          .AddTransient<IWideAvailabilitySearchService, WideAvailabilitySearchService>()
          .AddTransient<IBookingService, BookingService>()
          .AddTransient<ILocationService, LocationService>();

        builder.Services.AddTransient<AvailabilitySearchMapper>();

        builder.Services.AddTransient<TimezoneService>();

        builder.Services.AddTransient<AvailabilityRequestStorage>()
            .AddTransient<AvailabilitySearchResultStorage>()
            .AddTransient<PreBookResultStorage>();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<FastpayhotelsContext>();

        builder.Services.AddProblemDetailsErrorHandling()
            .ConfigureSwagger()
            .ConfigureDatabaseOptions(vaultClient, builder.Configuration)
            .ConfigureHttpClients(vaultClient, builder.Configuration);
    }
}
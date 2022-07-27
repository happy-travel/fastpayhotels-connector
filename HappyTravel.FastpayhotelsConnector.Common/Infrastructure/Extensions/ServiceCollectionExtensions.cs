using HappyTravel.FastpayhotelsConnector.Common.Service.TokenAuthHeader;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace HappyTravel.FastpayhotelsConnector.Common.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTokenAuthHeaderService(this IServiceCollection services, string tokenEndPoint)
        {             
            services.AddHttpClient(Constants.FastpayhotelsTokenClient, client =>
                {
                    client.BaseAddress = new Uri(tokenEndPoint);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetDefaultRetryPolicy());

            return services.AddTransient<TokenAuthHeaderClient>()
                .AddTransient<TokenAuthHeaderService>()
                .AddTransient<TokenAuthHeaderStorage>();


            IAsyncPolicy<HttpResponseMessage> GetDefaultRetryPolicy()
            {
                var jitter = new Random();

                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == HttpStatusCode.ServiceUnavailable)
                    .WaitAndRetryAsync(3, attempt
                        => TimeSpan.FromSeconds(Math.Pow(1.5, attempt)) + TimeSpan.FromMilliseconds(jitter.Next(0, 100)));
            }
        }
    }
}

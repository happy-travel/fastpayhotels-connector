using HappyTravel.FastpayhotelsConnector.Common.Service.TokenAuthHeader;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace HappyTravel.FastpayhotelsConnector.Common.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// The method adds the http client and services needed to get the token
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection</param>
        /// <param name="tokenEndPoint">tokenEndPoint</param>
        /// <returns></returns>
        public static IServiceCollection AddTokenAuthHeaderService(this IServiceCollection services, string tokenEndPoint)
        {
            var httpMessageHandlerLifeTimeMinutes = TimeSpan.FromMinutes(5);
            const int RetryCount = 3;

            services.AddHttpClient(Constants.FastpayhotelsTokenClient, client =>
                {
                    client.BaseAddress = new Uri(tokenEndPoint);
                })
                .SetHandlerLifetime(httpMessageHandlerLifeTimeMinutes)
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
                    .WaitAndRetryAsync(RetryCount, attempt
                        => TimeSpan.FromSeconds(Math.Pow(1.5, attempt)) + TimeSpan.FromMilliseconds(jitter.Next(0, 100)));
            }
        }
    }
}

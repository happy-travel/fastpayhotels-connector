using HappyTravel.FastpayhotelsConnector.Updater;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var environment = hostingContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true,
                        reloadOnChange: true);               

                config.AddEnvironmentVariables();
            })
                 .ConfigureLogging((hostingContext, logging) =>
                 {
                     
                 }).UseStartup<Startup>());


    public const string ConnectorUpdaterConsulName = "fastpayhotels-connector-updater";
}
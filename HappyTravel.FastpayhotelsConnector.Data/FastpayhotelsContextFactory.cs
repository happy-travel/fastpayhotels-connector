using HappyTravel.VaultClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace HappyTravel.FastpayhotelsConnector.Data;

internal class FastpayhotelsContextFactory : IDesignTimeDbContextFactory<FastpayhotelsContext>
{
    public FastpayhotelsContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
              .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
              .AddJsonFile("contextSettings.json", false, true)
              .Build();

        var dbOptions = GetDbOptions(configuration);

        var dbContextOptions = new DbContextOptionsBuilder<FastpayhotelsContext>();
        ((DbContextOptionsBuilder)dbContextOptions).UseNpgsql(GetConnectionString(dbOptions), builder => builder.UseNetTopologySuite());
        var context = new FastpayhotelsContext(dbContextOptions.Options);
        if (dbOptions.ContainsKey("migrationCommandTimeout"))
            context.Database.SetCommandTimeout(int.Parse(dbOptions["migrationCommandTimeout"]));

        return context;
    }


    private static string GetConnectionString(Dictionary<string, string> dbOptions)
    {
        return string.Format(ConnectionStringTemplate,
            dbOptions["host"],
            dbOptions["port"],
            dbOptions["userId"],
            dbOptions["password"]);
    }


    private static Dictionary<string, string> GetDbOptions(IConfiguration configuration)
    {
        var vaultUrl = Environment.GetEnvironmentVariable(configuration["Vault:Endpoint"]) ??
            throw new ArgumentException("Could not obtain Vault endpoint environment variables");

        using var vaultClient = new VaultClient.VaultClient(new VaultOptions
        {
            BaseUrl = new Uri(vaultUrl, UriKind.Absolute),
            Engine = configuration["Vault:Engine"],
            Role = configuration["Vault:Role"]
        });
        vaultClient.Login(Environment.GetEnvironmentVariable(configuration["Vault:Token"]), LoginMethods.Token)?.GetAwaiter().GetResult();
        return vaultClient.Get(configuration["Database:Options"]).GetAwaiter().GetResult();
    }


    private const string ConnectionStringTemplate = "Server={0};Port={1};Database=fastpayhotels;Userid={2};Password={3};";
}

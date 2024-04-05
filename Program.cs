using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Function.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<AppDbContext>(options => {
            // var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("KeyVaultUrl"));
            // var secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
            // var connectionString = secretClient.GetSecret("sql").Value.Value;
            // options.UseSqlServer(connectionString);

            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            options.UseSqlServer(connectionString);
        });
    })
    .Build();

host.Run();

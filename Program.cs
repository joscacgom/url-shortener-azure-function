using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Function.Data;
using Microsoft.EntityFrameworkCore;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<AppDbContext>(options =>
        {
            // Alternative with azure key vault
            // var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("KeyVaultUrl"));
            // var secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
            // var connectionString = secretClient.GetSecret("sql").Value.Value;
            // options.UseSqlServer(connectionString);

            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            options.UseSqlServer(connectionString);
        });
        services.AddHttpContextAccessor();
    })
    .Build();

host.Run();

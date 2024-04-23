using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Function.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<AppDbContext>(options =>
        {
            // var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("KeyVaultUrl"));
            // var secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
            // var connectionString = secretClient.GetSecret("sql").Value.Value;
            // options.UseSqlServer(connectionString);

            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            options.UseSqlServer(connectionString);
        });
        services.AddHttpContextAccessor();

        var config = context.Configuration;
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;


         services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                config.Bind("AzureAdB2C", options);
            }, options =>
            {
                config.Bind("AzureAdB2C", options);
            });

        services.AddAuthorization();
    })
    .Build();

host.Run();

using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using EScooter.ManageDigitalTwins;
using EScooter.ManageDigitalTwins.DigitalTwins;
using EScooter.ManageDigitalTwins.IotHub;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]

namespace EScooter.ManageDigitalTwins;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        ConfigureServices(builder.Services);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton(p =>
        {
            var digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            var httpClient = p.GetRequiredService<HttpClient>();
            return new DigitalTwinsClient(
                new Uri(digitalTwinUrl),
                credential,
                new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) });
        });
        services.AddSingleton(p =>
        {
            var connectionString = Environment.GetEnvironmentVariable("HubRegistryConnectionString");
            return RegistryManager.CreateFromConnectionString(connectionString);
        });

        services.AddSingleton<IDigitalTwinsService, AdtDigitalTwinsService>();
        services.AddSingleton<IScooterDevicesService, IotHubScooterDevicesService>();
    }
}

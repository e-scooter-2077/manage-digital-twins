// Default URL for triggering event grid function in the local environment.
//  https://3a9d-2001-171b-c9a4-7a71-994b-274e-22d1-d21.eu.ngrok.io/runtime/webhooks/EventGrid?functionName=manage-telemetry
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using EScooter.Monitor.ManageTelemetryUpdates;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EScooter.ScooterMonitor.ManageTelemetryUpdates
{
    /// <summary>
    /// A class containing a function to manage device telemetry updates.
    /// </summary>
    public class TelemetryManager
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");

        private readonly ILogger<TelemetryManager> _logger;

        public TelemetryManager(ILogger<TelemetryManager> log)
        {
            _logger = log;
        }

        [FunctionName("manage-telemetry")]
        public async Task ManageTelemetry([EventGridTrigger] TelemetryEvent input)
        {
            // Parse message
            var json = JObject.Parse(input.Data.ToString());
            var deviceId = json.Value<JObject>("systemProperties").Value<string>("iothub-connection-device-id");
            var telemetry = JsonConvert.DeserializeObject<ScooterTelemetry>(json.Value<JObject>("body").ToString());
            _logger.LogInformation(telemetry.ToString());

            // Update properties on DT
            var patch = TelemetryPatchFactory.CreatePatchDocument(telemetry);
            _logger.LogInformation($"Patch: {patch}");

            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(_digitalTwinUrl), credential, new DigitalTwinsClientOptions
            {
                Transport = new HttpClientTransport(_httpClient)
            });

            await digitalTwinsClient.UpdateDigitalTwinAsync(deviceId, patch);

            _logger.LogInformation($"Update telemetry on: {deviceId}");
        }
    }
}

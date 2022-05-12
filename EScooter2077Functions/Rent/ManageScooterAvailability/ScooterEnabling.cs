using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Rent.ManageScooterAvailability
{
    public record ScooterEnablingEvent(Guid ScooterId);

    public class ScooterEnabling
    {
        private static readonly HttpClient _httpClient = new();

        private readonly ILogger<ScooterEnabling> _logger;

        public ScooterEnabling(ILogger<ScooterEnabling> log)
        {
            _logger = log;
        }

        [FunctionName("manage-scooter-enabling")]
        public async Task SetEnabled([ServiceBusTrigger("%ServiceEventsTopicName%", "%SetEnabledSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg, IDictionary<string, object> userProperties)
        {
            var digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(digitalTwinUrl), credential, new DigitalTwinsClientOptions
            {
                Transport = new HttpClientTransport(_httpClient)
            });

            var enabled = userProperties["eventType"].ToString() == "ScooterEnabled";
            var message = JsonConvert.DeserializeObject<ScooterEnablingEvent>(mySbMsg);

            var patch = new JsonPatchDocument();
            patch.AppendReplace("/Enabled", enabled);

            await digitalTwinsClient.UpdateDigitalTwinAsync(message.ScooterId.ToString(), patch);
        }
    }
}

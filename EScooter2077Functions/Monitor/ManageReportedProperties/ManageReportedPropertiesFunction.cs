using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Monitor.ManageReportedProperties
{
    public class ManageReportedPropertiesFunction
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");

        private readonly ILogger<ManageReportedPropertiesFunction> _logger;

        public ManageReportedPropertiesFunction(ILogger<ManageReportedPropertiesFunction> log)
        {
            _logger = log;
        }

        [FunctionName("manage-properties")]
        public async Task ManageProperties([ServiceBusTrigger("%ServiceEventsTopicName%", "%ReportedPropertiesSubscriptionName%", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var scooterStatusChanged = JsonConvert.DeserializeObject<ScooterStatusChanged>(mySbMsg);
            var scooterId = scooterStatusChanged.Id;
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusChanged);

            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(_digitalTwinUrl), credential, new DigitalTwinsClientOptions
            {
                Transport = new HttpClientTransport(_httpClient)
            });

            await digitalTwinsClient.UpdateDigitalTwinAsync(scooterId, patch);
            _logger.LogInformation($"Updated reported properties of twin: {scooterId}\n");
        }
    }
}

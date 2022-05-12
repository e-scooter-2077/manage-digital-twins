using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Rent.ManageRents
{
    public record RentConfirmed(Guid RentId, Guid CustomerId, Guid ScooterId, DateTime Timestamp);

    public record RentCancelledOrStopped(Guid RentId, Guid CustomerId, Guid ScooterId);

    public class ManageRents
    {
        private static readonly HttpClient _httpClient = new();

        private readonly ILogger<ManageRents> _logger;

        public ManageRents(ILogger<ManageRents> log)
        {
            _logger = log;
        }

        private static DigitalTwinsClient CreateDigitalTwinsClient()
        {
            var digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            return new DigitalTwinsClient(new Uri(digitalTwinUrl), credential, new DigitalTwinsClientOptions
            {
                Transport = new HttpClientTransport(_httpClient)
            });
        }

        [FunctionName("add-rent")]
        public async Task AddRent([ServiceBusTrigger("%ServiceEventsTopicName%", "%AddRentSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var message = JsonConvert.DeserializeObject<RentConfirmed>(mySbMsg);
            var client = CreateDigitalTwinsClient();
            await DTUtils.CreateRentRelationship(
                message.RentId,
                message.CustomerId,
                message.ScooterId,
                message.Timestamp,
                client);

            _logger.LogInformation($"Add Rent: {mySbMsg}");
        }

        [FunctionName("remove-rent")]
        public async Task RemoveRent([ServiceBusTrigger("%ServiceEventsTopicName%", "%RemoveRentSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var message = JsonConvert.DeserializeObject<RentCancelledOrStopped>(mySbMsg);
            var client = CreateDigitalTwinsClient();

            await DTUtils.RemoveRentRelationship(
                message.RentId,
                message.CustomerId,
                client);

            _logger.LogInformation($"Remove rent: {mySbMsg}");
        }
    }
}

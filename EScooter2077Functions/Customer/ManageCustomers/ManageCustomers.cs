using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Customer.ManageCustomers
{
    public record CustomerCreated(Guid Id);

    public record CustomerDeleted(Guid Id);

    public class ManageCustomers
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");

        private readonly ILogger<ManageCustomers> _logger;

        public ManageCustomers(ILogger<ManageCustomers> log)
        {
            _logger = log;
        }

        private static DigitalTwinsClient InstantiateDtClient()
        {
            var credential = new DefaultAzureCredential();
            return new DigitalTwinsClient(
                        new Uri(_digitalTwinUrl),
                        credential,
                        new DigitalTwinsClientOptions { Transport = new HttpClientTransport(_httpClient) });
        }

        [FunctionName("add-customer")]
        public async Task AddCustomer([ServiceBusTrigger("%TopicName%", "%AddSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var digitalTwinsClient = InstantiateDtClient();

            var message = JsonConvert.DeserializeObject<CustomerCreated>(mySbMsg);
            try
            {
                await DTUtils.AddDigitalTwin(message.Id, digitalTwinsClient);
                _logger.LogInformation($"Add customer: {mySbMsg}");
            }
            catch (RequestFailedException e)
            {
                _logger.LogInformation($"Function failed to add customer {e.ErrorCode}");
            }
        }

        [FunctionName("remove-customer")]
        public async Task RemoveCustomer([ServiceBusTrigger("%TopicName%", "%RemoveSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var digitalTwinsClient = InstantiateDtClient();

            var message = JsonConvert.DeserializeObject<CustomerDeleted>(mySbMsg);

            try
            {
                await DTUtils.RemoveDigitalTwin(message.Id, digitalTwinsClient);
                _logger.LogInformation($"Removed customer: {mySbMsg}");
            }
            catch (RequestFailedException e)
            {
                _logger.LogInformation($"Function failed to remove customer {e.ErrorCode}");
            }
        }
    }
}

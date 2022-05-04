using Azure.Core;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using EScooter.DigitalTwins.Commons;
using System;
using System.Threading.Tasks;

namespace EScooter.PhysicalControl.ManageDevices
{
    public class DigitalTwinManager
    {
        private readonly DigitalTwinsClient _dtClient;

        public static DigitalTwinManager InstantiateDigitalTwinManager(System.Net.Http.HttpClient httpClient)
        {
            string dtUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            TokenCredential credentials = new DefaultAzureCredential();
            DigitalTwinManager dtManager = new DigitalTwinManager(new Uri(dtUrl), credentials, new DigitalTwinsClientOptions
            { Transport = new HttpClientTransport(httpClient) });
            return dtManager;
        }

        public DigitalTwinManager(Uri uri, Azure.Core.TokenCredential token, DigitalTwinsClientOptions digitalTwinsClientOptions)
        {
            _dtClient = new DigitalTwinsClient(uri, token, digitalTwinsClientOptions);
        }

        public async Task AddDigitalTwin(Guid id)
        {
            var twinData = new ScooterDigitalTwin(id);
            await _dtClient.CreateOrReplaceDigitalTwinAsync(twinData.Id, twinData);
        }

        public async Task RemoveDigitalTwin(Guid id)
        {
            await _dtClient.DeleteDigitalTwinAsync(id.ToString());
        }
    }
}

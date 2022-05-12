using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.PhysicalControl.ManageDevices
{
    public record ScooterCreated(Guid Id);

    public record ScooterDeleted(Guid Id);

    /// <summary>
    /// A function that adds a device to the IoTHub when the event is received.
    /// </summary>
    public class ManageDevices
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly DigitalTwinManager _dtManager = DigitalTwinManager.InstantiateDigitalTwinManager(_httpClient);
        private static readonly IoTHubManager _iotHubManager = IoTHubManager.InstantiateIoTHubManager();

        private readonly ILogger<ManageDevices> _logger;

        public ManageDevices(ILogger<ManageDevices> log)
        {
            _logger = log;
        }

        [FunctionName("add-device-and-twin")]
        public async Task AddDevice([ServiceBusTrigger("%ServiceEventsTopicName%", "%AddDeviceSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var message = JsonConvert.DeserializeObject<ScooterCreated>(mySbMsg);

            // Add Digital Twin first
            await _dtManager.AddDigitalTwin(message.Id);

            // Then add IoTHub Device
            var (device, exists) = await _iotHubManager.AddOrGetDeviceAsync(message.Id);
            if (exists)
            {
                _logger.LogInformation($"Device with id {device.Id} already existing");
            }
            else
            {
                _logger.LogInformation($"New device registered with id {device.Id}");
                await _iotHubManager.SetDefaultProperties(message.Id);
                _logger.LogInformation($"Update device twin default properties");
            }
        }

        [FunctionName("remove-device-and-twin")]
        public async Task RemoveDevice([ServiceBusTrigger("%ServiceEventsTopicName%", "%RemoveDeviceSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var message = JsonConvert.DeserializeObject<ScooterCreated>(mySbMsg);
            await _dtManager.RemoveDigitalTwin(message.Id);
            await _iotHubManager.RemoveDevice(message.Id);
            _logger.LogInformation($"Device with id {message.Id} was removed");
        }
    }
}

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading.Tasks;

namespace EScooter.PhysicalControl.ManageDevices
{
    public class IoTHubManager
    {
        private readonly RegistryManager _registryManager;

        public static IoTHubManager InstantiateIoTHubManager()
        {
            string iotHubString = Environment.GetEnvironmentVariable("HubRegistryConnectionString");
            IoTHubManager iotHubManager = new IoTHubManager(iotHubString);
            return iotHubManager;
        }

        public IoTHubManager(string connectionString)
        {
            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async Task<(Device Device, bool Exists)> AddOrGetDeviceAsync(Guid id)
        {
            Device device;
            bool exists = false;
            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(id.ToString()));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(id.ToString());
                exists = true;
            }

            return (device, exists);
        }

        public async Task RemoveDevice(Guid id)
        {
            await _registryManager.RemoveDeviceAsync(id.ToString());
        }

        public async Task<Twin> SetDefaultProperties(Guid id)
        {
            var twin = await _registryManager.GetTwinAsync(id.ToString());
            var patch =
                @"{
                    tags: {
                        type: 'EScooter'
                    }
                }";
            return await _registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
        }
    }
}

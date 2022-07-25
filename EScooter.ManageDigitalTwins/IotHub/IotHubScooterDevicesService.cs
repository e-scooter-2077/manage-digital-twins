using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace EScooter.ManageDigitalTwins.IotHub;

public class IotHubScooterDevicesService : IScooterDevicesService
{
    private readonly RegistryManager _registryManager;

    public IotHubScooterDevicesService(RegistryManager registryManager)
    {
        _registryManager = registryManager;
    }

    public async Task CreateScooterDevice(Guid id)
    {
        try
        {
            await _registryManager.AddDeviceAsync(new Device(id.ToString()));
        }
        catch (DeviceAlreadyExistsException)
        {
        }
    }

    public async Task RemoveScooterDevice(Guid id)
    {
        await _registryManager.RemoveDeviceAsync(id.ToString());
    }
}

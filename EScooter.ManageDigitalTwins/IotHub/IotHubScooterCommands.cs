using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace EScooter.ManageDigitalTwins.IotHub;

public class IotHubScooterCommands
{
    private readonly RegistryManager _registryManager;

    public IotHubScooterCommands(RegistryManager registryManager)
    {
        _registryManager = registryManager;
    }

    public async Task CreateScooter(Guid id)
    {
        try
        {
            await _registryManager.AddDeviceAsync(new Device(id.ToString()));
        }
        catch (DeviceAlreadyExistsException)
        {
        }
    }

    public async Task RemoveScooter(Guid id)
    {
        await _registryManager.RemoveDeviceAsync(id.ToString());
    }
}

using System;
using System.Threading.Tasks;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using static EScooter.ManageDigitalTwins.Functions.ConfigurationKeys;

namespace EScooter.ManageDigitalTwins.Functions;

public class ManageScooters
{
    private readonly IDigitalTwinsService _digitalTwinsService;
    private readonly IScooterDevicesService _scooterDevicesService;

    public ManageScooters(IDigitalTwinsService digitalTwinsService, IScooterDevicesService scooterDevicesService)
    {
        _digitalTwinsService = digitalTwinsService;
        _scooterDevicesService = scooterDevicesService;
    }

    private record ScooterCreated(Guid Id);

    private record ScooterDeleted(Guid Id);

    [FunctionName("add-device-and-twin")]
    public async Task AddDeviceAndTwin(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.AddDevice, Connection = ServiceBusConnection)] string messageBody)
    {
        var message = JsonConvert.DeserializeObject<ScooterCreated>(messageBody);
        await _scooterDevicesService.CreateScooterDevice(message.Id);
        await _digitalTwinsService.CreateScooter(message.Id);
    }

    [FunctionName("remove-device-and-twin")]
    public async Task RemoveDeviceAndTwin(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.RemoveDevice, Connection = ServiceBusConnection)] string messageBody)
    {
        var message = JsonConvert.DeserializeObject<ScooterDeleted>(messageBody);
        await _scooterDevicesService.RemoveScooterDevice(message.Id);
        await _digitalTwinsService.RemoveScooter(message.Id);
    }
}

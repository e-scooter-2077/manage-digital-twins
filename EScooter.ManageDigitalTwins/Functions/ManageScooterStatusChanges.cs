using System;
using System.Threading.Tasks;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using static EScooter.ManageDigitalTwins.Functions.ConfigurationKeys;

namespace EScooter.ManageDigitalTwins.Functions;

public class ManageScooterStatusChanges
{
    private readonly IDigitalTwinsService _digitalTwinsService;

    public ManageScooterStatusChanges(IDigitalTwinsService digitalTwinsService)
    {
        _digitalTwinsService = digitalTwinsService;
    }

    private record ScooterStatusChanged(
        Guid Id,
        bool? Locked,
        double? MaxSpeed,
        bool? Standby);

    [FunctionName("manage-status-changes")]
    public async Task ManageStatusChanges(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.StatusChanged, Connection = ServiceBusConnection)] string messageBody)
    {
        var message = JsonConvert.DeserializeObject<ScooterStatusChanged>(messageBody);
        await _digitalTwinsService.UpdateScooter(
            message.Id,
            connected: true,
            standby: message.Standby,
            maxSpeed: message.MaxSpeed,
            locked: message.Locked);
    }
}

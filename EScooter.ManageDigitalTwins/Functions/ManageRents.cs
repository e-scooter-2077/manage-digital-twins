using System;
using System.Threading.Tasks;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using static EScooter.ManageDigitalTwins.Functions.ConfigurationKeys;

namespace EScooter.ManageDigitalTwins.Functions;

public class ManageRents
{
    private readonly IDigitalTwinsService _digitalTwinsService;

    public ManageRents(IDigitalTwinsService digitalTwinsService)
    {
        _digitalTwinsService = digitalTwinsService;
    }

    private record RentConfirmed(Guid RentId, Guid CustomerId, Guid ScooterId, DateTime Timestamp);

    private record RentCancelledOrStopped(Guid RentId, Guid CustomerId, Guid ScooterId);

    [FunctionName("add-rent")]
    public async Task AddRent(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.AddRent, Connection = ServiceBusConnection)] string messageBody)
    {
        var message = JsonConvert.DeserializeObject<RentConfirmed>(messageBody);
        await _digitalTwinsService.StartRent(message.RentId, message.ScooterId, message.CustomerId, message.Timestamp);
    }

    [FunctionName("remove-rent")]
    public async Task RemoveRent(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.RemoveRent, Connection = ServiceBusConnection)] string messageBody)
    {
        var message = JsonConvert.DeserializeObject<RentCancelledOrStopped>(messageBody);
        await _digitalTwinsService.StopRent(message.RentId, message.CustomerId);
    }
}

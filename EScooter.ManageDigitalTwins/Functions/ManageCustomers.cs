using System;
using System.Threading.Tasks;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using static EScooter.ManageDigitalTwins.Functions.ConfigurationKeys;

namespace EScooter.ManageDigitalTwins.Functions;

public class ManageCustomers
{
    private readonly IDigitalTwinsService _digitalTwinsService;

    public ManageCustomers(IDigitalTwinsService digitalTwinsService)
    {
        _digitalTwinsService = digitalTwinsService;
    }

    private record CustomerCreated(Guid Id, string Username);

    private record CustomerDeleted(Guid Id);

    [FunctionName("add-customer")]
    public async Task AddCustomer(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.AddCustomer, Connection = ServiceBusConnection)] string messageBody)
    {
        var message = JsonConvert.DeserializeObject<CustomerCreated>(messageBody);
        await _digitalTwinsService.CreateCustomer(message.Id, message.Username);
    }

    [FunctionName("remove-customer")]
    public async Task RemoveCustomer(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.RemoveCustomer, Connection = ServiceBusConnection)] string messageBody)
    {
        var message = JsonConvert.DeserializeObject<CustomerDeleted>(messageBody);
        await _digitalTwinsService.RemoveCustomer(message.Id);
    }
}

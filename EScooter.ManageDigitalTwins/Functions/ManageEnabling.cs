using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using static EScooter.ManageDigitalTwins.Functions.ConfigurationKeys;

namespace EScooter.ManageDigitalTwins.Functions;

public class ManageEnabling
{
    private const string ScooterEnabledEvent = "ScooterEnabled";
    private const string ScooterDisabledEvent = "ScooterDisabled";

    private readonly IDigitalTwinsService _digitalTwinsService;

    public ManageEnabling(IDigitalTwinsService digitalTwinsService)
    {
        _digitalTwinsService = digitalTwinsService;
    }

    private record ScooterEnablingEvent(Guid ScooterId);

    [FunctionName("manage-scooter-enabling")]
    public async Task SetEnabled(
        [ServiceBusTrigger(ServiceEventsTopic, Sub.SetEnabled, Connection = ServiceBusConnection)] string messageBody,
        IDictionary<string, object> userProperties)
    {
        var enabled = userProperties["eventType"].ToString() switch
        {
            ScooterEnabledEvent => true,
            ScooterDisabledEvent => false,
            _ => throw new NotSupportedException()
        };

        var message = JsonConvert.DeserializeObject<ScooterEnablingEvent>(messageBody);

        await _digitalTwinsService.UpdateScooter(message.ScooterId, enabled: enabled);
    }
}

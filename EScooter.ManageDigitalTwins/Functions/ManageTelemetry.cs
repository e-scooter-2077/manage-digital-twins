using System;
using System.Threading.Tasks;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json.Linq;

namespace EScooter.ManageDigitalTwins.Functions;
public class ManageTelemetry
{
    private readonly IDigitalTwinsService _digitalTwinsService;

    public ManageTelemetry(IDigitalTwinsService digitalTwinsService)
    {
        _digitalTwinsService = digitalTwinsService;
    }

    private record ScooterTelemetry(double BatteryLevel, double Speed, double Latitude, double Longitude);

    [FunctionName("manage-telemetry")]
    public async Task UpdateTelemetry([EventGridTrigger] JObject input)
    {
        var data = input.Value<JObject>("data");
        var scooterId = data.Value<JObject>("systemProperties").Value<Guid>("iothub-connection-device-id");
        var telemetry = data.ToObject<ScooterTelemetry>();

        await _digitalTwinsService.UpdateScooter(
            scooterId,
            connected: true,
            speed: telemetry.Speed,
            batteryLevel: telemetry.BatteryLevel,
            latitude: telemetry.Latitude,
            longitude: telemetry.Latitude);
    }
}

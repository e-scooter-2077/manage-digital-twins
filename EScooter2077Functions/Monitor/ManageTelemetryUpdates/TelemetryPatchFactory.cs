using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Monitor.ManageTelemetryUpdates
{
    public static class TelemetryPatchFactory
    {
        public static JsonPatchDocument CreatePatchDocument(ScooterTelemetry telemetry)
        {
            var patch = new JsonPatchDocument();
            patch.AppendReplace("/Connected", true);
            patch.AppendReplace("/BatteryLevel", telemetry.BatteryLevel);
            patch.AppendReplace("/Speed", ConverterUtils.ConvertSpeed(telemetry.Speed));
            patch.AppendReplace("/Latitude", telemetry.Latitude);
            patch.AppendReplace("/Longitude", telemetry.Longitude);
            return patch;
        }
    }
}

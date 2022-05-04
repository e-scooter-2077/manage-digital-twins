using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Monitor.ManageTelemetryUpdates
{
    public record ScooterTelemetry(double BatteryLevel, double Speed, double Latitude, double Longitude);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Monitor.ManageTelemetryUpdates
{
    public static class ConverterUtils
    {
        public static double ConvertSpeed(double speedInMS)
        {
            return Math.Round(speedInMS * 3.6, 4);
        }
    }
}

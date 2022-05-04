using System;

namespace EScooter.Monitor.ManageTelemetryUpdates
{
    public record TelemetryEvent(string Id, string Topic, string Subject, string EventType, DateTime EventTime, object Data);
}

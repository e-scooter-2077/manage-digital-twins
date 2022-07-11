namespace EScooter.Monitor.ManageReportedProperties
{
    public record ScooterStatusChanged(
        string Id,
        bool? Locked,
        string UpdateFrequency,
        double? MaxSpeed,
        bool? Standby);
}

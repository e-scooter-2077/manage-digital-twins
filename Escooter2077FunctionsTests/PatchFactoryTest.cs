using Azure;
using Shouldly;
using Xunit;

namespace EScooter.Monitor.ManageTelemetryUpdates.UnitTests;

public class PatchFactoryTest
{
    [Fact]
    public void TestPatchFactory()
    {
        ScooterTelemetry telemetry = new ScooterTelemetry(100.0, 10, 44.133331, 12.233333);
        JsonPatchDocument patch = TelemetryPatchFactory.CreatePatchDocument(telemetry);
        string expected = "[{'op':'replace','path':'/Connected','value':true}," +
            "{'op':'replace','path':'/BatteryLevel','value':100}," +
            "{'op':'replace','path':'/Speed','value':36}," +
            "{'op':'replace','path':'/Latitude','value':44.133331}," +
            "{'op':'replace','path':'/Longitude','value':12.233333}]";
        expected = expected.Replace('\'', '\"');
        patch.ToString().ShouldBe(expected);
    }
}

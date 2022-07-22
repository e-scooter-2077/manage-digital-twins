using Shouldly;
using Xunit;

namespace EScooter.Monitor.ManageTelemetryUpdates.UnitTests;

public class ConversionTest
{
    [Fact]
    public void ConvertSpeed()
    {
        double speedInKmH = ConverterUtils.ConvertSpeed(10);
        speedInKmH.ShouldBe(36);
    }

    [Fact]
    public void ConvertNegativeSpeed()
    {
        double speedInKmH = ConverterUtils.ConvertSpeed(-10);
        speedInKmH.ShouldBe(-36);
    }

    [Fact]
    public void ConvertDecimalSpeed()
    {
        double speedInKmH = ConverterUtils.ConvertSpeed(6.7);
        speedInKmH.ShouldBe(24.12);
    }
}

using Azure.DigitalTwins.Core;
using System;
using System.Text.Json.Serialization;

namespace EScooter.DigitalTwins.Commons
{
    public class ScooterDigitalTwin
    {
        [JsonPropertyName(DigitalTwinsJsonPropertyNames.DigitalTwinId)]
        public string Id { get; set; }

        [JsonPropertyName(DigitalTwinsJsonPropertyNames.DigitalTwinETag)]
        public string ETag { get; set; }

        [JsonPropertyName(DigitalTwinsJsonPropertyNames.DigitalTwinMetadata)]
        public ScooterDigitalTwinMetadata Metadata { get; set; }

        [JsonPropertyName("UpdateFrequency")]
        public int UpdateFrequency { get; set; }

        [JsonPropertyName("Locked")]
        public bool Locked { get; set; }

        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("MaxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonPropertyName("Standby")]
        public bool Standby { get; set; }

        [JsonPropertyName("Connected")]
        public bool Connected { get; set; }

        [JsonPropertyName("BatteryLevel")]
        public double BatteryLevel { get; set; }

        [JsonPropertyName("Latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("Longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("Speed")]
        public double Speed { get; set; }

        public ScooterDigitalTwin() : this(Guid.NewGuid())
        {
        }

        public ScooterDigitalTwin(Guid id)
        {
            Id = id.ToString();
            Connected = false;
            Locked = true;
            BatteryLevel = 0;
            Enabled = false;
            Standby = true;
            UpdateFrequency = 30;
            MaxSpeed = 30;
            Metadata = new ScooterDigitalTwinMetadata();
        }
    }

    public class ScooterDigitalTwinMetadata
    {
        [JsonPropertyName(DigitalTwinsJsonPropertyNames.MetadataModel)]
        public string ModelId { get; }

        public ScooterDigitalTwinMetadata()
        {
            ModelId = "dtmi:com:escooter:EScooter;1";
        }
    }
}

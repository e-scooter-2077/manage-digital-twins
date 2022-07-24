namespace EScooter.ManageDigitalTwins.DigitalTwins;

public static class Models
{
    public static class Scooter
    {
        public const string Id = "dtmi:com:escooter:EScooter;1";

        public const string LatitudeProperty = "Latitude";

        public const string LongitudeProperty = "Longitude";

        public const string BatteryLevelProperty = "BatteryLevel";

        public const string SpeedProperty = "Speed";

        public const string MaxSpeedProperty = "MaxSpeed";

        public const string EnabledProperty = "Enabled";

        public const string LockedProperty = "Locked";

        public const string StandbyProperty = "Standby";

        public const string ConnectedProperty = "Connected";
    }

    public static class Customer
    {
        public const string Id = "dtmi:com:escooter:Customer;1";

        public const string UsernameProperty = "Username";

        public const string IsRidingRelationship = "is_riding";
    }
}

namespace EScooter.ManageDigitalTwins.Functions;

public static class ConfigurationKeys
{
    public const string ServiceBusConnection = "ServiceBusConnectionString";

    public const string ServiceEventsTopic = "%ServiceEventsTopic%";

    public static class Sub
    {
        public const string AddDevice = "%AddDeviceSubscription%";

        public const string RemoveDevice = "%RemoveDeviceSubscription%";

        public const string AddCustomer = "%AddCustomerSubscription%";

        public const string RemoveCustomer = "%RemoveCustomerSubscription%";

        public const string StatusChanged = "%ReportedPropertiesSubscription%";

        public const string SetEnabled = "%SetEnabledSubscription%";

        public const string AddRent = "%AddRentSubscription%";

        public const string RemoveRent = "%RemoveRentSubscription%";
    }
}

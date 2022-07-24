using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EScooter.ManageDigitalTwins.Services;

public record CustomerDto(
    Guid Id,
    string Username);

public record ScooterDto(
    Guid Id,
    double Latitude,
    double Longitude,
    double BatteryLevel,
    double MaxSpeed,
    double Speed,
    bool Enabled,
    bool Rented,
    bool Locked,
    bool Standby,
    bool Connected);

public interface IDigitalTwinsService
{
    Task<IEnumerable<CustomerDto>> GetCustomers();

    Task CreateCustomer(Guid id, string username);

    Task RemoveCustomer(Guid id);

    Task<IEnumerable<ScooterDto>> GetScooters();

    Task CreateScooter(Guid id);

    Task RemoveScooter(Guid id);

    Task UpdateScooter(
        Guid id,
        bool? locked = null,
        bool? enabled = null,
        bool? standby = null,
        bool? connected = null,
        double? maxSpeed = null,
        double? speed = null,
        double? batteryLevel = null,
        double? latitude = null,
        double? longitude = null);

    Task StartRent(Guid rentId, Guid scooterId, Guid customerId, DateTime timestamp);

    Task StopRent(Guid rentId, Guid customerId);
}

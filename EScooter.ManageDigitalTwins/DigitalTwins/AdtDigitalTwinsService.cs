using Azure;
using Azure.DigitalTwins.Core;
using EasyDesk.Tools.Collections;
using EScooter.ManageDigitalTwins.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EScooter.ManageDigitalTwins.DigitalTwins;

public class RentedScooterResultDto
{
    [JsonPropertyName("target")]
    public BasicDigitalTwin Target { get; set; }
}

public class AdtDigitalTwinsService : IDigitalTwinsService
{
    private readonly DigitalTwinsClient _client;

    public AdtDigitalTwinsService(DigitalTwinsClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<ScooterDto>> GetScooters()
    {
        var query = $"SELECT * FROM DIGITALTWINS DT WHERE IS_OF_MODEL(DT, '{Models.Scooter.Id}')";
        var scooters = await _client.ListQuery<BasicDigitalTwin>(query);

        if (scooters.IsEmpty())
        {
            return Enumerable.Empty<ScooterDto>();
        }

        var idString = scooters.Select(x => $"'{x.Id}'").ConcatStrings(", ");
        var queryRents = $"SELECT target FROM DIGITALTWINS source JOIN target RELATED source.{Models.Customer.IsRidingRelationship} WHERE target.$dtId IN [{idString}]";

        var rentedScooters = await _client.ListQuery<RentedScooterResultDto>(queryRents);
        return scooters.GroupJoin(rentedScooters, x => x.Id, y => y.Target.Id, (s, r) => ToScooterDto(s, r.Any()));
    }

    private ScooterDto ToScooterDto(BasicDigitalTwin twin, bool rented) => new(
        Id: Guid.Parse(twin.Id),
        Latitude: twin.ReadProperty(Models.Scooter.LatitudeProperty).GetDouble(),
        Longitude: twin.ReadProperty(Models.Scooter.LongitudeProperty).GetDouble(),
        BatteryLevel: twin.ReadProperty(Models.Scooter.BatteryLevelProperty).GetDouble(),
        MaxSpeed: twin.ReadProperty(Models.Scooter.MaxSpeedProperty).GetDouble(),
        Speed: twin.ReadProperty(Models.Scooter.SpeedProperty).GetDouble(),
        Enabled: twin.ReadProperty(Models.Scooter.EnabledProperty).GetBoolean(),
        Rented: rented,
        Locked: twin.ReadProperty(Models.Scooter.LockedProperty).GetBoolean(),
        Standby: twin.ReadProperty(Models.Scooter.StandbyProperty).GetBoolean(),
        Connected: twin.ReadProperty(Models.Scooter.ConnectedProperty).GetBoolean());

    public async Task CreateScooter(Guid id)
    {
        await _client.CreateTwin(id, Models.Scooter.Id, twin =>
        {
            twin.Contents.Add(Models.Scooter.ConnectedProperty, false);
            twin.Contents.Add(Models.Scooter.LockedProperty, true);
            twin.Contents.Add(Models.Scooter.BatteryLevelProperty, 0);
            twin.Contents.Add(Models.Scooter.EnabledProperty, false);
            twin.Contents.Add(Models.Scooter.StandbyProperty, true);
            twin.Contents.Add(Models.Scooter.MaxSpeedProperty, 30);
            twin.Contents.Add(Models.Scooter.SpeedProperty, 0);
            twin.Contents.Add(Models.Scooter.LatitudeProperty, 0);
            twin.Contents.Add(Models.Scooter.LongitudeProperty, 0);
        });
    }

    public async Task RemoveScooter(Guid id)
    {
        await _client.DeleteTwin(id);
    }

    public async Task UpdateScooter(
        Guid id,
        bool? locked = null,
        bool? enabled = null,
        bool? standby = null,
        bool? connected = null,
        double? maxSpeed = null,
        double? speed = null,
        double? batteryLevel = null,
        double? latitude = null,
        double? longitude = null)
    {
        var patch = new JsonPatchDocument();
        patch.AppendReplaceIfPresent(Models.Scooter.LockedProperty, locked);
        patch.AppendReplaceIfPresent(Models.Scooter.StandbyProperty, standby);
        patch.AppendReplaceIfPresent(Models.Scooter.EnabledProperty, enabled);
        patch.AppendReplaceIfPresent(Models.Scooter.ConnectedProperty, connected);
        patch.AppendReplaceIfPresent(Models.Scooter.BatteryLevelProperty, batteryLevel);
        patch.AppendReplaceIfPresent(Models.Scooter.SpeedProperty, speed);
        patch.AppendReplaceIfPresent(Models.Scooter.LatitudeProperty, latitude);
        patch.AppendReplaceIfPresent(Models.Scooter.LongitudeProperty, longitude);

        await _client.UpdateDigitalTwinAsync(id.ToString(), patch);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomers()
    {
        var query = $"SELECT * FROM DIGITALTWINS DT WHERE IS_OF_MODEL(DT, '{Models.Customer.Id}')";
        var customerTwins = await _client.ListQuery<BasicDigitalTwin>(query);
        return customerTwins.Select(ToCustomerDto);
    }

    private CustomerDto ToCustomerDto(BasicDigitalTwin twin) => new(
        Id: Guid.Parse(twin.Id),
        Username: twin.ReadProperty(Models.Customer.UsernameProperty).GetString());

    public async Task CreateCustomer(Guid id, string username)
    {
        await _client.CreateTwin(id, Models.Customer.Id, twin =>
        {
            twin.Contents.Add(Models.Customer.UsernameProperty, username);
        });
    }

    public async Task RemoveCustomer(Guid id)
    {
        await _client.DeleteTwin(id);
    }

    public async Task StartRent(Guid rentId, Guid scooterId, Guid customerId, DateTime timestamp)
    {
        var relationship = new BasicRelationship
        {
            TargetId = scooterId.ToString(),
            Name = Models.Customer.IsRidingRelationship,
            Properties = new Dictionary<string, object>()
            {
                { "start", timestamp }
            }
        };
        await _client.CreateOrReplaceRelationshipAsync(customerId.ToString(), rentId.ToString(), relationship);
    }

    public async Task StopRent(Guid rentId, Guid customerId)
    {
        await _client.DeleteRelationshipAsync(customerId.ToString(), rentId.ToString());
    }
}

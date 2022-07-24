using System;
using System.Threading.Tasks;

namespace EScooter.ManageDigitalTwins.Services;

public interface IScooterDevicesService
{
    Task CreateScooterDevice(Guid id);

    Task RemoveScooterDevice(Guid id);
}

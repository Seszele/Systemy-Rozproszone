using Grpc.Core;
using MyGrpcService;
using Smarthome;

namespace MyGrpcService.Services;

public class DeviceManagerService : DeviceManager.DeviceManagerBase
{
    private readonly List<DeviceInfo> _devices = new List<DeviceInfo>
        {
            new DeviceInfo { Id = "1", Type = "TemperatureSensor" },
            // new DeviceInfo { Id = "2", Type = "TemperatureSensor" },
            new DeviceInfo { Id = "2", Type = "Camera" },
            // Add more devices as needed
        };

    public override Task<ListDevicesResponse> ListDevices(ListDevicesRequest request, ServerCallContext context)
    {
        var response = new ListDevicesResponse();
        response.Devices.AddRange(_devices);
        return Task.FromResult(response);
    }

    public override Task<DeviceStatusResponse> GetDeviceStatus(DeviceStatusRequest request, ServerCallContext context)
    {
        var status = "Online";

        var response = new DeviceStatusResponse
        {
            Id = request.Id,
            Status = status
        };

        return Task.FromResult(response);
    }
}
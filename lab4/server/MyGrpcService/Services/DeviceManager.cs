using Grpc.Core;
using MyGrpcService;
using Smarthome;

namespace MyGrpcService.Services;
public enum DeviceType
{
    TemperatureSensor,
    Camera,
    SmartSpeaker
}
public class DeviceManagerService : DeviceManager.DeviceManagerBase
{
    private readonly List<DeviceInfo> _devices = new List<DeviceInfo>
    {
        new DeviceInfo { Id = "1", Type = DeviceType.TemperatureSensor.ToString() },
        new DeviceInfo { Id = "2", Type = DeviceType.Camera.ToString() },
        new DeviceInfo { Id = "3", Type = DeviceType.SmartSpeaker.ToString() },
        new DeviceInfo { Id = "4", Type = DeviceType.SmartSpeaker.ToString() },
    };

    private readonly Dictionary<string, SpeakerSubtype> _speakerSubtypes = new Dictionary<string, SpeakerSubtype>
    {
        { "3", new SpeakerSubtype { Basic = new BasicSpeaker() } },
        { "4", new SpeakerSubtype { Bluetooth = new BluetoothSpeaker { BluetoothAddress = "00:11:22:33:44:55" } } },
    };

    public override Task<ListDevicesResponse> ListDevices(ListDevicesRequest request, ServerCallContext context)
    {
        var response = new ListDevicesResponse();
        response.Devices.AddRange(_devices.Select(device => new DeviceInfo
        {
            Id = device.Id,
            Type = device.Type,
            SpeakerSubtype = device.Type == DeviceType.SmartSpeaker.ToString() ? _speakerSubtypes[device.Id] : null
        }));
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

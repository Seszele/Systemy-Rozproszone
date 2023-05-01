using Grpc.Core;
using MyGrpcService;
using Smarthome;

namespace MyGrpcService.Services;

public class TemperatureSensorService : TemperatureSensor.TemperatureSensorBase
{
    private readonly ILogger<TemperatureSensorService> _logger;
    private static Dictionary<string, float> _sensors = new Dictionary<string, float>
        {
            { "1", 22.5f },
            { "2", 24.3f },
            { "6", 24.3f },
        };
    public TemperatureSensorService(ILogger<TemperatureSensorService> logger)
    {
        _logger = logger;
    }
    public override Task<TemperatureResponse> GetTemperature(TemperatureRequest request, ServerCallContext context)
    {
        _sensors["1"] += Random.Shared.Next(-1, 2) * (Random.Shared.NextSingle());
        _sensors["2"] += Random.Shared.Next(-1, 2) * (Random.Shared.NextSingle());
        _sensors["6"] += Random.Shared.Next(-1, 2) * (Random.Shared.NextSingle());
        if (_sensors.TryGetValue(request.Id, out float temperature))
        {
            return Task.FromResult(new TemperatureResponse { Temperature = temperature });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"Temperature sensor with ID '{request.Id}' not found."));
    }
}
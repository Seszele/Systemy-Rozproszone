using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Smarthome;
namespace MyGrpcService.Services;
public class SmartSpeakerService : SmartSpeaker.SmartSpeakerBase
{
    public struct SpeakerInfo
    {
        public int Volume { get; set; }
        public bool Playing { get; set; }
    }
    private readonly ILogger<SmartSpeakerService> _logger;
    private readonly Dictionary<string, SpeakerInfo> _volumes = new Dictionary<string, SpeakerInfo>();

    public SmartSpeakerService(ILogger<SmartSpeakerService> logger)
    {
        _logger = logger;
        _volumes = new Dictionary<string, SpeakerInfo>
            {
                { "3", new SpeakerInfo { Volume = 50, Playing = false } },
                { "4", new SpeakerInfo{Volume = 75, Playing = true }},
            };
    }

    public override Task<SpeakerVolumeResponse> GetVolume(SpeakerRequest request, ServerCallContext context)
    {
        var response = new SpeakerVolumeResponse();
        if (_volumes.TryGetValue(request.Id, out SpeakerInfo speakerInfo))
        {
            response.Id = request.Id;
            response.Volume = speakerInfo.Volume;
        }
        return Task.FromResult(response);
    }

    public override Task<Empty> SetVolume(SpeakerVolumeRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Setting volume for speaker {request.Id} to {request.Volume}");
        _volumes[request.Id] = new SpeakerInfo { Volume = request.Volume, Playing = _volumes[request.Id].Playing };
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> PlayAudio(SpeakerRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Playing audio on speaker {request.Id}");
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> PauseAudio(SpeakerRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Pausing audio on speaker {request.Id}");
        return Task.FromResult(new Empty());
    }
    public override Task<AudioStatusResponse> GetAudioStatus(SpeakerRequest request, ServerCallContext context)
    {
        var response = new AudioStatusResponse();
        response.Id = request.Id;
        response.Playing = _volumes[request.Id].Playing;
        return Task.FromResult(response);
    }
}

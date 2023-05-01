using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Smarthome;

namespace MyGrpcService.Services;

public class CameraService : Camera.CameraBase
{
    private readonly ILogger<CameraService> _logger;
    private readonly Dictionary<string, CameraState> _cameras;

    public CameraService(ILogger<CameraService> logger)
    {
        _logger = logger;
        _cameras = new Dictionary<string, CameraState>
            {
                { "1", new CameraState() },
                { "2", new CameraState() },
            };
    }

    public override Task<ImageResponse> GetImage(ImageRequest request, ServerCallContext context)
    {
        if (_cameras.TryGetValue(request.Id, out CameraState cameraState))
        {
            // Load the sample image and apply the current camera state (pan, tilt, zoom)
            using var image = Image.Load("./image.jpg");
            ApplyCameraState(image, cameraState);

            // Convert the modified image to a byte array
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new JpegEncoder());
            byte[] imageData = memoryStream.ToArray();

            return Task.FromResult(new ImageResponse { Id = request.Id, ImageData = Google.Protobuf.ByteString.CopyFrom(imageData) });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"Camera with ID '{request.Id}' not found."));
    }

    public override Task<PTZControlResponse> ControlPTZ(PTZControlRequest request, ServerCallContext context)
    {
        if (_cameras.TryGetValue(request.Id, out CameraState cameraState))
        {
            // Update the camera state based on the PTZ control
            switch (request.Control)
            {
                case PTZControl.PanLeft:
                    cameraState.Pan -= 10;
                    break;
                case PTZControl.PanRight:
                    cameraState.Pan += 10;
                    break;
                case PTZControl.TiltUp:
                    cameraState.Tilt += 10;
                    break;
                case PTZControl.TiltDown:
                    cameraState.Tilt -= 10;
                    break;
                case PTZControl.ZoomIn:
                    cameraState.Zoom *= 1.1f;
                    break;
                case PTZControl.ZoomOut:
                    cameraState.Zoom /= 1.1f;
                    break;
            }

            string status = $"PTZ control '{request.Control}' executed for camera '{request.Id}' camera zoom: {cameraState.Zoom}";
            return Task.FromResult(new PTZControlResponse { Id = request.Id, Success = true });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"Camera with ID '{request.Id}' not found."));
    }

    private void ApplyCameraState(Image image, CameraState cameraState)
    {
        _logger.LogInformation($"Applying camera state: pan={cameraState.Pan}, tilt={cameraState.Tilt}, zoom={cameraState.Zoom}");

        int zoomedWidth = (int)(image.Width / cameraState.Zoom);
        int zoomedHeight = (int)(image.Height / cameraState.Zoom);

        int pan = Math.Clamp(cameraState.Pan + (image.Width - zoomedWidth) / 2, 0, image.Width - zoomedWidth);
        int tilt = Math.Clamp(cameraState.Tilt + (image.Height - zoomedHeight) / 2, 0, image.Height - zoomedHeight);

        // Apply pan, tilt, and zoom
        image.Mutate(x => x
            .Crop(new Rectangle(pan, tilt, zoomedWidth, zoomedHeight))
            .Resize(new ResizeOptions
            {
                Size = new Size(image.Width, image.Height),
                Sampler = new NearestNeighborResampler(),
                Mode = ResizeMode.Max
            }));
    }

}

public class CameraState
{
    public int Pan { get; set; }
    public int Tilt { get; set; }
    private float _zoom = 1.0f;
    public float Zoom
    {
        get => _zoom;
        set => _zoom = Math.Clamp(value, 1f, 10.0f);
    }

    public CameraState()
    {
        Pan = 0;
        Tilt = 0;
        Zoom = 1.0f;
    }
}



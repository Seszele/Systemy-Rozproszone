using MyGrpcService.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
// builder.Services.AddSingleton<CameraService>();
builder.Services.AddSingleton<SmartSpeakerService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<TemperatureSensorService>();
app.MapGrpcService<DeviceManagerService>();
app.MapGrpcService<CameraService>();
app.MapGrpcService<SmartSpeakerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// cors
app.UseCors("AllowAllOrigins");
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.Run();

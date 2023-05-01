import React, { useEffect, useMemo, useState } from 'react';
import grpcTransport from './grpc/transport';
import './App.css';
import { DeviceManagerClient, TemperatureSensorClient, CameraClient, SmartSpeakerClient } from './grpc/server/MyGrpcService/Protos/smarthome.client';
import { DeviceInfo, ListDevicesRequest } from './grpc/server/MyGrpcService/Protos/smarthome';
import TemperatureSensor from './components/temperatureSensor/TemperatureSensor';
import Camera from './components/camera/Camera';
import SmartSpeaker from './components/smartSpeaker/SmartSpeaker';
import { Toaster } from 'react-hot-toast';

const GreeterComponent: React.FC = () => {
  const [devices, setDevices] = useState<DeviceInfo[]>([]);

  const deviceManagerClient = useMemo(() => {
    return new DeviceManagerClient(grpcTransport);
  }, [grpcTransport]);
  const temperatureSensorClient = useMemo(() => {
    return new TemperatureSensorClient(grpcTransport);
  }, [grpcTransport]);
  const cameraClient = useMemo(() => {
    return new CameraClient(grpcTransport);
  }, [grpcTransport]);
  const smartSpeakerClient = useMemo(() => {
    return new SmartSpeakerClient(grpcTransport);
  }, [grpcTransport]);

  useEffect(() => {
    async function fetchDevices() {
      const request = ListDevicesRequest.create({});
      const response = await deviceManagerClient.listDevices(request).response;
      console.log(response.devices);

      setDevices(response.devices);
    }

    fetchDevices();
  }, [deviceManagerClient]);

  return (
    <div >
      <Toaster></Toaster>
      <h1>Intelligent Home</h1>
      <div className='flex flex-col gap-3'>

        {devices.map((device) => {
          switch (device.type) {
            case "TemperatureSensor":
              return <TemperatureSensor key={device.id} id={device.id} client={temperatureSensorClient} />;
            case "Camera":
              return <Camera key={device.id} id={device.id} client={cameraClient} />;
            case "SmartSpeaker":
              return <SmartSpeaker key={device.id} id={device.id} client={smartSpeakerClient} speakerSubtype={device.speakerSubtype!} />;
            default:
              return <div key={device.id}>Unknown device type: {device.type}</div>;
          }
        })}
      </div>

    </div>
  );
};

export default GreeterComponent;

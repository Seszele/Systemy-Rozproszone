import React, { useEffect, useState } from 'react';
import { CameraClient } from '../../grpc/server/MyGrpcService/Protos/smarthome.client';
import { ImageRequest, PTZControl, PTZControlRequest } from '../../grpc/server/MyGrpcService/Protos/smarthome';
import { Buffer } from 'buffer';
import ZoomInIcon from '@mui/icons-material/ZoomIn';
import ZoomOutIcon from '@mui/icons-material/ZoomOut';

interface CameraProps {
  id: string;
  client: CameraClient;
}

const Camera: React.FC<CameraProps> = ({ id, client }) => {
  const [imageData, setImageData] = useState<Uint8Array | null>(null);

  useEffect(() => {

    fetchImage();
  }, [id, client]);

  async function fetchImage() {
    const request = ImageRequest.create({ id });
    const response = await client.getImage(request).response;

    setImageData(response.imageData);
  }
  async function controlPTZ(control: PTZControl) {
    const request = PTZControlRequest.create({ id, control });
    await client.controlPTZ(request);
    fetchImage();
  }

  return (
    <div className="p-4 bg-gradient-to-r from-gray-800 to-blue-900 rounded-md shadow-md">
      <h2 className="text-lg font-bold mb-2">Camera {id}</h2>
      {imageData ? (
        <img
          src={`data:image/jpeg;base64,${Buffer.from(imageData).toString('base64')}`}
          alt={`Camera ${id}`}
          className="w-full h-96 object-cover rounded-md"
        />
      ) : (
        <div className="w-full h-48 bg-gray-200 rounded-md flex items-center justify-center">
          <span className='text-black'>Loading image...</span>
        </div>
      )}
      <div className="flex justify-between mt-4">
        <button
          onClick={() => controlPTZ(PTZControl.PAN_LEFT)}
          className="bg-blue-500 text-white px-4 py-2 rounded-md"
        >
          Pan Left
        </button>
        <button
          onClick={() => controlPTZ(PTZControl.PAN_RIGHT)}
          className="bg-blue-500 text-white px-4 py-2 rounded-md"
        >
          Pan Right
        </button>
        <button
          onClick={() => controlPTZ(PTZControl.TILT_UP)}
          className="bg-blue-500 text-white px-4 py-2 rounded-md"
        >
          Tilt Up
        </button>
        <button
          onClick={() => controlPTZ(PTZControl.TILT_DOWN)}
          className="bg-blue-500 text-white px-4 py-2 rounded-md"
        >
          Tilt Down
        </button>
        <button
          onClick={() => controlPTZ(PTZControl.ZOOM_IN)}
          className="bg-blue-500 text-white px-4 py-2 rounded-md disabled"
        >
          <ZoomInIcon></ZoomInIcon>
        </button>
        <button
          onClick={() => controlPTZ(PTZControl.ZOOM_OUT)}
          className="bg-blue-500 text-white px-4 py-2 rounded-md"
        >
          <ZoomOutIcon></ZoomOutIcon>
        </button>
      </div>
    </div>
  );
};

export default Camera;

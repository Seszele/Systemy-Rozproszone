import React, { useState, useEffect } from "react";
import { TemperatureSensorClient } from "../../grpc/server/MyGrpcService/Protos/smarthome.client"
import { TemperatureRequest } from "../../grpc/server/MyGrpcService/Protos//smarthome";

interface TemperatureSensorProps {
  id: string;
  client: TemperatureSensorClient;
}

const TemperatureSensor: React.FC<TemperatureSensorProps> = ({ id, client }) => {
  const [temperature, setTemperature] = useState<number | null>(null);

  useEffect(() => {
    fetchTemperature();    
  }, [id, client]);

  async function fetchTemperature() {
    console.log("fetching temperature");

    const request = TemperatureRequest.create({id});
    const response = await client.getTemperature(request).response;
    setTemperature(response.temperature)
  }

  return (
    <div>
      <h3>Temperature Sensor {id}</h3>
      <p>Temperature: {temperature !== null ? `${temperature.toFixed(2)} °C` : "Loading..."}</p>
      <button onClick={() => fetchTemperature()}>Refresh</button>
    </div>
  );
};

export default TemperatureSensor;

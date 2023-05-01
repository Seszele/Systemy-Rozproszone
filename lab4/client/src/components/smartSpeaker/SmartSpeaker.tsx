import React, { useState, useEffect } from 'react';
import { SpeakerRequest, SpeakerSubtype, SpeakerVolumeRequest } from '../../grpc/server/MyGrpcService/Protos/smarthome';
import { SmartSpeakerClient } from '../../grpc/server/MyGrpcService/Protos/smarthome.client';
import PauseIcon from '@mui/icons-material/Pause';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import BluetoothIcon from '@mui/icons-material/Bluetooth';
import SpeakerIcon from '@mui/icons-material/Speaker';
import toast, { Toaster } from 'react-hot-toast';

interface SmartSpeakerProps {
    id: string;
    client: SmartSpeakerClient;
    speakerSubtype: SpeakerSubtype;
}
const SmartSpeaker: React.FC<SmartSpeakerProps> = ({ id, client, speakerSubtype }) => {
    const [volume, setVolume] = useState<number>(0);
    const [audioStatus, setAudioStatus] = useState<boolean>(false);

    useEffect(() => {
        async function fetchVolume() {
            const request = SpeakerRequest.create({ id });
            const response = await client.getVolume(request).response;
            setVolume(response.volume);
        }
        async function fetchAudioStatus() {
            const request = SpeakerRequest.create({ id });
            const response = await client.getAudioStatus(request).response;
            setAudioStatus(response.playing);
        }

        fetchVolume();
        fetchAudioStatus();
    }, [id, client]);

    const handleVolumeChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const newVolume = parseInt(event.target.value, 10);
        setVolume(newVolume);

        const request = SpeakerVolumeRequest.create({ id, volume: newVolume });
        await client.setVolume(request).response;
    };
    const handleAudioStatusChange = async () => {
        const request = SpeakerRequest.create({ id });
        if (audioStatus)
            await client.pauseAudio(request).response;
        else
            await client.playAudio(request).response;
        setAudioStatus(!audioStatus);

    };

    return (
        <div className="flex flex-col items-center">
            {speakerSubtype.subtype.oneofKind === "bluetooth"
                ? <button onClick={() => toast("Current bluetooth adress\n" + speakerSubtype.subtype.bluetooth.bluetoothAddress)}>
                    <BluetoothIcon></BluetoothIcon>
                </button>
                : <SpeakerIcon></SpeakerIcon>}
            <h2>Smart Speaker: {id} {speakerSubtype.subtype.oneofKind}</h2>
            <div>
                <label htmlFor={`volume-${id}`}>Volume: </label>
                <input
                    type="range"
                    id={`volume-${id}`}
                    name={`volume-${id}`}
                    min="0"
                    max="100"
                    value={volume}
                    onChange={handleVolumeChange}
                />
                <span>{volume}</span>
            </div>
            <button onClick={handleAudioStatusChange} className='w-24 bg-blue-500 '>
                {audioStatus ? <PauseIcon></PauseIcon> : <PlayArrowIcon></PlayArrowIcon>}
            </button>
        </div>
    );
};

export default SmartSpeaker;


import { GrpcWebFetchTransport } from '@protobuf-ts/grpcweb-transport';

// Replace 'http://localhost:5000' with the URL of your gRPC server
const grpcTransport = new GrpcWebFetchTransport({
  baseUrl: 'https://localhost:5288'
});

export default grpcTransport;

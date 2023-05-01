import { GrpcWebFetchTransport } from '@protobuf-ts/grpcweb-transport';

const grpcTransport = new GrpcWebFetchTransport({
  baseUrl: 'https://localhost:5288'
});

export default grpcTransport;

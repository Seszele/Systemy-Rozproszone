@echo off
client\node_modules\.bin\protoc-gen-ts.cmd %*
@REM protoc --ts_out ./client/src/grpc --ts_opt generate_dependencies --plugin ./protoc-gen-ts.cmd ./server/MyGrpcService/Protos/greet.proto
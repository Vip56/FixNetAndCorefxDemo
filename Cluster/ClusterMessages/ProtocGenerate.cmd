echo off
protoc -I . --csharp_out . --csharp_opt=file_extension=.g.cs --grpc_out . --plugin=protoc-gen-grpc=grpc_csharp_plugin.exe Protos.proto

echo Ω· ¯±‡“Î
pause

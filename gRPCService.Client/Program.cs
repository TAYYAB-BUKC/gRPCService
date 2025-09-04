// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using gRPCService.Basics;

Console.WriteLine("Hello, World!");

string SERVER_URL = "https://localhost:7106/";
var channelOptions = new GrpcChannelOptions()
{

};

using var channel = GrpcChannel.ForAddress(SERVER_URL, channelOptions);

var client = new FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient(channel);
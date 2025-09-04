// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using gRPCService.Basics;

string SERVER_URL = "https://localhost:7106/";
var channelOptions = new GrpcChannelOptions()
{

};

using var channel = GrpcChannel.ForAddress(SERVER_URL, channelOptions);

var client = new FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient(channel);

ConsumeUnaryMethod(client);

Console.ReadKey(true);

void ConsumeUnaryMethod(FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client)
{
	var request = new Request() { Content = "Hello gRPC Server" };
	var response  = client.Unary(request);
	Console.WriteLine(response.Message);
}
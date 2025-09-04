// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using gRPCService.Basics;

string SERVER_URL = "https://localhost:7106/";
var channelOptions = new GrpcChannelOptions()
{

};

using var channel = GrpcChannel.ForAddress(SERVER_URL, channelOptions);

var client = new FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient(channel);

//ConsumeUnaryMethod(client);

//ConsumeClientStreamingMethod(client);

ConsumeServerStreamingMethod(client);

ConsumeBiDirectionalStreamingMethod(client);

Console.ReadKey(true);

void ConsumeUnaryMethod(FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client)
{
	var request = new Request() { Content = "Hello gRPC Server" };
	var response  = client.Unary(request, deadline: DateTime.UtcNow.AddMicroseconds(500));
	Console.WriteLine(response.Message);
}

async void ConsumeClientStreamingMethod(FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client)
{
	using var request = client.ClientStreaming();
	for (var i = 1; i <= 100; i++)
	{
		//Console.WriteLine($"Client says: Hello to the gRPC Server {i} times.");
		await request.RequestStream.WriteAsync(new Request()
		{
			Content = $"Client says: Hello to the gRPC Server {i} times."
		});
	}

	await request.RequestStream.CompleteAsync();
	var response = await request;
	Console.WriteLine(response.Message);
}

async void ConsumeServerStreamingMethod(FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client)
{
	var request = client.ServerStreaming(new Request()
	{
		Content = "Hello gRPC Server"
	});

	await foreach (var response in request.ResponseStream.ReadAllAsync())
	{
		Console.WriteLine(response.Message);
	}
}

async void ConsumeBiDirectionalStreamingMethod(FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client)
{
	using (var request = client.BiDirectionalStreaming())
	{
		for (int i = 1; i <= 20; i++)
		{
			await request.RequestStream.WriteAsync(new Request()
			{
				Content = $"Client: Hello to the gRPC Server {i} times."
			});
		}

		await request.RequestStream.CompleteAsync();

		while (await request.ResponseStream.MoveNext())
		{
			Console.WriteLine(request.ResponseStream.Current.Message);
		}
	}
}
// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Grpc.Reflection.V1Alpha;
using gRPCService.Basics;
using Microsoft.Extensions.DependencyInjection;
using static Grpc.Core.Metadata;
using static Grpc.Reflection.V1Alpha.ServerReflection;

var retryPolicy = new MethodConfig()
{
	Names = { MethodName.Default },
	RetryPolicy = new RetryPolicy()
	{
		MaxAttempts = 5,
		BackoffMultiplier = 1,
		InitialBackoff = TimeSpan.FromSeconds(5),
		MaxBackoff = TimeSpan.FromSeconds(25),
		RetryableStatusCodes = { StatusCode.Internal }
	}
};

var hedgingPolicy = new MethodConfig()
{
	Names = { MethodName.Default },
	HedgingPolicy = new HedgingPolicy()
	{
		MaxAttempts = 5,
		HedgingDelay = TimeSpan.FromSeconds(5),
		NonFatalStatusCodes = { StatusCode.Internal },
	}
};

string SERVER_URL = "https://localhost:7106/";

using var serverReflectionChannel = GrpcChannel.ForAddress(SERVER_URL);
var serverReflectionClient = new ServerReflectionClient(serverReflectionChannel);

ServerReflectionRequest request = new ServerReflectionRequest() { ListServices = "" };
using var actualRequest = serverReflectionClient.ServerReflectionInfo();
await actualRequest.RequestStream.WriteAsync(request);
await actualRequest.RequestStream.CompleteAsync();

while (await actualRequest.ResponseStream.MoveNext())
{
	var response = actualRequest.ResponseStream.Current;
	foreach (var service in response.ListServicesResponse.Service)
	{
		Console.WriteLine($"- {service.Name}");
	}
}

var channelOptions = new GrpcChannelOptions()
{
	ServiceConfig = new ServiceConfig()
	{
		//MethodConfigs = { retryPolicy }
		MethodConfigs = { hedgingPolicy }
	}
};

var serversFactory = new StaticResolverFactory(options => new[]
{
	new BalancerAddress("localhost", 7106),
	new BalancerAddress("localhost", 7107),
});

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<ResolverFactory>(serversFactory);

using var channel = GrpcChannel.ForAddress(SERVER_URL, channelOptions);
//using var channel = GrpcChannel.ForAddress("static://localhost", new GrpcChannelOptions()
//{
//	//Credentials = ChannelCredentials.Insecure,
//	Credentials = ChannelCredentials.SecureSsl,
//	ServiceProvider = serviceCollection.BuildServiceProvider(),
//	ServiceConfig = new ServiceConfig()
//	{
//		LoadBalancingConfigs = { new RoundRobinConfig() },
//	}
//});

var healthClient = new Health.HealthClient(channel);
var healthResult = await healthClient.CheckAsync(new HealthCheckRequest());
Console.WriteLine($"Health Status: {healthResult.Status}");

var client = new FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient(channel);

ConsumeUnaryMethod(client);

//ConsumeClientStreamingMethod(client);

//ConsumeServerStreamingMethod(client);

//ConsumeBiDirectionalStreamingMethod(client);

Console.ReadKey(true);

void ConsumeUnaryMethod(FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client)
{
	var metaData = new Metadata()
	{
		{ "grpc-accept-encoding", "gzip" }
	};
	var request = new Request() { Content = "Hello gRPC Server" };
	//var response  = client.Unary(request, deadline: DateTime.UtcNow.AddMicroseconds(500));
	var response = client.Unary(request, deadline: DateTime.UtcNow.AddMinutes(5));
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
	var cancellationToken = new CancellationTokenSource();
	var metadata = new Metadata();
	metadata.Add("my-first-key", "my-first-value");
	metadata.Add("my-second-key", "my-second-value");
	metadata.Add(new Entry("my-third-key", "my-third-value"));
	var request = client.ServerStreaming(new Request()
	{
		Content = "Hello gRPC Server"
	},
	headers: metadata);

	try
	{
		await foreach (var response in request.ResponseStream.ReadAllAsync(cancellationToken.Token))
		{
			Console.WriteLine(response.Message);
			if (response.Message.Contains("25"))
			{
				//cancellationToken.Cancel();
			}
		}
	}
	catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
	{

	}
	catch (RpcException ex) when (ex.StatusCode == StatusCode.PermissionDenied)
	{

	}
	catch (Exception ex)
	{
		
	}
	finally
	{
		var trailers = request.GetTrailers();
		var individualTrailer = trailers.GetValue("my-first-trailer");
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
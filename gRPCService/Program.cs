using Grpc.Net.Compression;
using gRPCService.Interceptors;
using gRPCService.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.ComponentModel;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
	options.Interceptors.Add<ServerLoggerInterceptor>();
	options.ResponseCompressionAlgorithm = "gzip";
	options.ResponseCompressionLevel = CompressionLevel.SmallestSize;
	// Custom Provider Registration
	//options.CompressionProviders = new List<ICompressionProvider>
	//{
	//	new GzipCompressionProvider(CompressionLevel.SmallestSize)
	//};
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<FirstGRPCService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

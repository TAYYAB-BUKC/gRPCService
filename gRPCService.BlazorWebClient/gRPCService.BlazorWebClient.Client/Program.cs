using Grpc.Net.Client.Web;
using gRPCService.Basics;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddGrpcClient<FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient>(options =>
{
	options.Address = new Uri("https://localhost:7106/");
}).ConfigureChannel(options =>
{
	options.HttpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
});

await builder.Build().RunAsync();

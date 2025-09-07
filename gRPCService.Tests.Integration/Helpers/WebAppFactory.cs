using Grpc.Net.Client;
using gRPCService.Basics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace gRPCService.Tests.Integration.Helpers
{
	public class WebAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("gRPCService.Tests.Integration");
			builder.ConfigureTestServices(services =>
			{

			});
			builder.UseTestServer();
		}

		public FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient CreateGrpcClient()
		{
			var httpClient = CreateClient();
			var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
			{
				HttpClient = httpClient
			});

			return new FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient(channel);
		}
	}
}

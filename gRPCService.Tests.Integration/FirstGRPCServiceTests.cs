using FluentAssertions;
using gRPCService.Basics;
using gRPCService.Tests.Integration.Helpers;

namespace gRPCService.Tests.Integration
{
	public class FirstGRPCServiceTests : IClassFixture<WebAppFactory<Program>>
	{
		private readonly WebAppFactory<Program> webAppFactory;

		public FirstGRPCServiceTests(WebAppFactory<Program> webAppFactory)
		{
			this.webAppFactory = webAppFactory;
		}

		[Fact]
		public async Task Unary_ShoulReturnMessage()
		{
			// Arrange
			var client = webAppFactory.CreateGrpcClient();
			var request = new Request()
			{
				Content = "Hello gRPC Server"
			};
			var expectedResponse = new Response()
			{
				Message = "Server(localhost) received this content in the request: \nHello gRPC Server"
			};
			
			// Act
			var actualResponse = client.Unary(request);

			// Assert
			actualResponse.Should().BeEquivalentTo(expectedResponse);
		}
	}
}

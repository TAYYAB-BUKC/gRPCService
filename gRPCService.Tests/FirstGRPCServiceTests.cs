using FluentAssertions;
using gRPCService.Basics;
using gRPCService.Services;
using gRPCService.Tests.Helpers;

namespace gRPCService.Tests
{
	public class FirstGRPCServiceTests
	{
		private readonly IFirstGRPCService _gRPCService;
		public FirstGRPCServiceTests()
		{
			_gRPCService = new FirstGRPCService();
		}

		[Fact]
		public async Task Unary_ShoulReturnMessage()
		{
			// Arrange
			var request = new Request()
			{
				Content = "Hello gRPC Server"
			};
			var expectedResponse = new Response()
			{
				Message = "Server(Host Name) received this content in the request: \nHello gRPC Server"
			};

			// Act
			var response = await _gRPCService.Unary(request, TestServerCallContext.Create());

			// Assert
			response.Should().BeEquivalentTo(expectedResponse);
		}
	}
}
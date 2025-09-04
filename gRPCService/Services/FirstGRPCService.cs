using Grpc.Core;
using gRPCService.Basics;

namespace gRPCService.Services
{
	public class FirstGRPCService : FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionBase
	{
		public override Task<Response> Unary(Request request, ServerCallContext context)
		{
			return Task.FromResult(new Response()
			{
				Message = $"Server received this content in the request: \n\n{request.Content}"
			});
		}
	}
} 
using Grpc.Core;
using gRPCService.Basics;
using System.Text;

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

		public override async Task<Response> ClientStreaming(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
		{
			StringBuilder builder = new StringBuilder();
			Response response = new();
			builder.AppendLine("Server received these contents in the request: \n");
			while (await requestStream.MoveNext())
			{
				builder.AppendLine(requestStream.Current.Content);
			}
			response.Message = Convert.ToString(builder);
			return response;
		}

		public override async Task ServerStreaming(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
		{
			for (int i = 1; i <= 100; i++)
			{
				await responseStream.WriteAsync(new Response()
				{
					Message = $"{i}) Server received this content in the request: \n{request.Content}"
				});
			}
		}
	}
} 
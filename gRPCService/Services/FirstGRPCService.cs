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
			var firstHeader = context.RequestHeaders.Get("my-first-key");
			var secondHeader = context.RequestHeaders.Get("my-second-key");
			var firstHeaderValue = firstHeader is not null ? firstHeader.Value : null;
			var secondHeaderValue = secondHeader is not null ? secondHeader.Value : null;

			for (int i = 1; i <= 100; i++)
			{
				if (context.CancellationToken.IsCancellationRequested)
					return;

				await responseStream.WriteAsync(new Response()
				{
					Message = $"{i}) Server received this content in the request: \n{request.Content}"
				});
			}

			var trailer = new Metadata.Entry("my-first-trailer", "my-first-trailer-value");
			context.ResponseTrailers.Add(trailer);
		}

		public override async Task BiDirectionalStreaming(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
		{
			while (await requestStream.MoveNext())
			{
				await responseStream.WriteAsync(new Response()
				{
					Message = requestStream.Current.Content
				});
			}
		}
	}
} 
using Grpc.Core;
using gRPCService.Basics;

namespace gRPCService.Services
{
	public interface IFirstGRPCService
	{
		Task BiDirectionalStreaming(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context);
		Task<Response> ClientStreaming(IAsyncStreamReader<Request> requestStream, ServerCallContext context);
		Task ServerStreaming(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context);
		Task<Response> Unary(Request request, ServerCallContext context);
	}
}
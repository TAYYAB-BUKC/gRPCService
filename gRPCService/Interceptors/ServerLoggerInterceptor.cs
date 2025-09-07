using Grpc.Core;
using Grpc.Core.Interceptors;

namespace gRPCService.Interceptors
{
	public class ServerLoggerInterceptor : Interceptor
	{
		private readonly ILogger<ServerLoggerInterceptor> _logger;
		public ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger)
		{
			_logger = logger;
		}

		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
			TRequest request,
			ServerCallContext context,
			UnaryServerMethod<TRequest, TResponse> continuation)
		{
			try
			{
				_logger.LogInformation($"Server is now going to execute {context.Method}, {context.Status}");
				return await continuation(request, context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error thrown by {context.Method}");
				throw;
			}
		}
	}
}
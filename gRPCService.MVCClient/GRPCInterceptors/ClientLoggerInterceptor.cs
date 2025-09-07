using Grpc.Core.Interceptors;

namespace gRPCService.MVCClient.GRPCInterceptors
{
	public class ClientLoggerInterceptor : Interceptor
	{
		private readonly ILogger<ClientLoggerInterceptor> logger;
		public ClientLoggerInterceptor(ILoggerFactory loggerFactory)
		{
			logger = loggerFactory.CreateLogger<ClientLoggerInterceptor>();
		}

		public override TResponse BlockingUnaryCall<TRequest, TResponse>(
			TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			try
			{
				logger.LogInformation($"Starting the client call of type: {context.Method.FullName}, {context.Method.Type}");
				return continuation(request, context);
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}

using Grpc.Core;

namespace gRPCService.Tests.Helpers
{
	public class TestServerCallContext : ServerCallContext
	{
		private readonly Metadata metadata;
		private readonly CancellationToken cancellationToken;

		private TestServerCallContext(Metadata metadata, CancellationToken cancellationToken)
		{
			this.metadata = metadata;
			this.cancellationToken = cancellationToken;
		}

		protected override string MethodCore => "Method Name";

		protected override string HostCore => "Host Name";

		protected override string PeerCore => "Peer Name";

		protected override DateTime DeadlineCore { get; }

		protected override Metadata RequestHeadersCore => throw new NotImplementedException();

		protected override CancellationToken CancellationTokenCore => throw new NotImplementedException();

		protected override Metadata ResponseTrailersCore => throw new NotImplementedException();

		protected override Status StatusCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		protected override WriteOptions? WriteOptionsCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		protected override AuthContext AuthContextCore => throw new NotImplementedException();

		protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
		{
			throw new NotImplementedException();
		}

		protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
		{
			throw new NotImplementedException();
		}

		public static TestServerCallContext Create(Metadata? requestHeaders = null, CancellationToken cancellationToken = default)
		{
			return new TestServerCallContext(requestHeaders ?? new Metadata(), cancellationToken);
		}
	}
}
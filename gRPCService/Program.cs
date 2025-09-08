using Grpc.Net.Compression;
using gRPCService.Auth;
using gRPCService.Interceptors;
using gRPCService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IO.Compression;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
	options.Interceptors.Add<ServerLoggerInterceptor>();
	options.ResponseCompressionAlgorithm = "gzip";
	options.ResponseCompressionLevel = CompressionLevel.SmallestSize;
	// Custom Provider Registration
	//options.CompressionProviders = new List<ICompressionProvider>
	//{
	//	new GzipCompressionProvider(CompressionLevel.SmallestSize)
	//};
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.RequireHttpsMetadata = false;
					options.SaveToken = true;
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateAudience = false,
						ValidateIssuer = false,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = JwtHelper.SecurityKey,
					};
				});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("DefaultPolicy", policy =>
	{
		policy.RequireClaim(ClaimTypes.Name);
		policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
	});
});

builder.Services.AddHealthChecks().AddCheck("gRPCService", () => HealthCheckResult.Healthy(), new[] { "grpc", "live" });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<FirstGRPCService>();
app.MapGrpcHealthChecksService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program { }
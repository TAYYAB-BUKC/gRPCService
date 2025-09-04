using gRPCService.Basics;
using gRPCService.MVCClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace gRPCService.MVCClient.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client;

		public HomeController(ILogger<HomeController> logger, FirstGRPCServiceDefinition.FirstGRPCServiceDefinitionClient client)
		{
			_logger = logger;
			this.client = client;
		}

		public IActionResult Index()
		{
			var response = client.Unary(new Request() { Content = "Hello from the gRPC MVC Client to the gRPC Server." });
			ViewBag.Message = response.Message;
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

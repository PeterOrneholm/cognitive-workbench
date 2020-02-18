using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Orneholm.CognitiveWorkbench.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/computer-vision")]
        public IActionResult ComputerVision()
        {
            return View();
        }

        [Route("/face")]
        public IActionResult Face()
        {
            return View();
        }
    }
}

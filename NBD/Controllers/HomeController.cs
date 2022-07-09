using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBD.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBD.Controllers

{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ComputerContext db;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            db = new ComputerContext();
        }

        public async Task<ActionResult> Index(ComputerFilter filter)
        {
            var computers = await db.GetComputers(filter.Year, filter.ComputerName);
            var model = new ComputerList { Computers = computers, Filter = filter };
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Computer computer)
        {
            if (ModelState.IsValid)
            {
                await db.Create(computer);
                return RedirectToAction("Index");
            }
            return View(computer);
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
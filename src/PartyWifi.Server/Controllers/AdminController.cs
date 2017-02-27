using Microsoft.AspNetCore.Mvc;

namespace PartyWifi.Server.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
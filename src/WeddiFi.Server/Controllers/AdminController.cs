using Microsoft.AspNetCore.Mvc;

namespace WeddiFi.Server.Controllers
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
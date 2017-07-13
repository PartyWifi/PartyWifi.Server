using Microsoft.AspNetCore.Mvc;

namespace PartyWifi.Server.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

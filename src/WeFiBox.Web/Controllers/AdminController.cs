using Microsoft.AspNetCore.Mvc;

namespace WeFiBox.Web.Controllers
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
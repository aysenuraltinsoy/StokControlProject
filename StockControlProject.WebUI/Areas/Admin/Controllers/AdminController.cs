using Microsoft.AspNetCore.Mvc;

namespace StockControlProject.WebUI.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

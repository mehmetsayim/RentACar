using Microsoft.AspNetCore.Mvc;

namespace RentACarWeb.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

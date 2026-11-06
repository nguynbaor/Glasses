using Microsoft.AspNetCore.Mvc;

namespace Glasses.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        [Area("Admin")]
        public IActionResult UserIndex()
        {
            return View();
        }
    }
}

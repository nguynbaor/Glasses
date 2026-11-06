using Microsoft.AspNetCore.Mvc;

namespace Glasses.Areas.Admin.Controllers
{
    public class SettingController : Controller
    {
        [Area("Admin")]
        public IActionResult CaiDat()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forest.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    //[Authorize(Roles ="Admin,Editor,Moderator,Creator")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

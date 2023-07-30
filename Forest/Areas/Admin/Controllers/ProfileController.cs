using Forest.Data;
using Microsoft.AspNetCore.Mvc;

namespace Forest.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forest.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    //[Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Empsearch)
        {
            ViewData["Gettagnames"] = Empsearch;
            var empquery = from x in _roleManager.Roles select x;
            if (!String.IsNullOrEmpty(Empsearch))
            {
                empquery = empquery.Where(x => x.Name.Contains(Empsearch));
            }
            return View(await empquery.AsNoTracking().ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var checkrole = await _roleManager.FindByNameAsync(identityRole.Name);
            if(checkrole != null) {
                ModelState.AddModelError("Error", "This role name exist");
                return View();  
            }
            await _roleManager.CreateAsync(identityRole);
            return RedirectToAction("Index","Role");
        }
        
    }
}

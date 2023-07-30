using Forest.Areas.Admin.ViewModels;
using Forest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forest.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    //[Authorize(Roles = "Admin,Moderator")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public IActionResult Index()
        {
            var users=_userManager.Users.ToList();
            return View(users);
        }
        public async Task<IActionResult> AddRole(string id)
        {
            if(id == null) return NotFound();
            User user=await _userManager.FindByIdAsync(id);
            if(user==null) return NotFound();
            var userRoles = (await _userManager.GetRolesAsync(user)).ToList();
            var roles = _roleManager.Roles.Select(x => x.Name).ToList();

            UserRoleVM userRoleVM = new()
            {
                User = user,
                Roles = roles.Except(userRoles)
            };
            return View(userRoleVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string id,string role)
        {
            if(id == null) return NotFound();
            User user = await _userManager.FindByIdAsync(id);
            if(user==null) return NotFound();
            if (role == null)
            {
                ModelState.AddModelError("Error", "Something went wrong");
                return View();
            }
            var userAddRole=await _userManager.AddToRoleAsync(user, role);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(string userid)
        {
            if (userid == null) return NotFound();
            User user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound();

            return View(user);
        }

        public async Task<IActionResult> Delete(string userid, string role)
        {
            if (userid == null || role == null) return NotFound();

            User user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound();

            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Something went wrong!";
                return View();
            }
            return RedirectToAction("Index");
        }
    }
}

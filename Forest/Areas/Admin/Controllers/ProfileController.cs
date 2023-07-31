using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Forest.Data;
using Forest.Models;
using Forest.Data;
using Microsoft.PowerBI.Api;

namespace ForestAppUI.Controllers
{
    [Area(nameof(Admin))]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;

        public ProfileController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, AppDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = await _userManager.FindByIdAsync(userId);

            ViewData["User"] = user;

            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                User user = await _userManager.FindByIdAsync(userId);

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                user.PhotoUrl = "/uploads/" + fileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    User user = await _userManager.FindByIdAsync(userId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.AboutAuthor = model.AboutAuthor;
                    user.Email = model.Email;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "An error occurred while updating the profile.");
                }
            }

            return View("Index", model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string renewPassword)
        {
            if (newPassword != renewPassword)
            {
                ModelState.AddModelError("", "New password and confirmation password do not match.");
                return RedirectToAction("Index");
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View("Index", user);
        }
    }
}

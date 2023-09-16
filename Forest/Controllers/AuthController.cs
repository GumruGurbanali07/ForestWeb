using Forest.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Forest.DTOs;
using Forest.Models;


namespace WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginDTO loginDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(loginDto);
        //    }

        //    var checkEmail = await _userManager.FindByEmailAsync(loginDto.Email);

        //    if (checkEmail == null)
        //    {
        //        ModelState.AddModelError("Error", "Email or Password is not valid!");
        //        return View(loginDto);
        //    }

        //    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(checkEmail, loginDto.Password, loginDto.RememberMe, true);
        //    if (!result.Succeeded)
        //    {
        //        ModelState.AddModelError("Error", "Email or Password is not valid!");
        //        return View(loginDto);
        //    }
        //    else
        //    {
        //        var c = _httpContextAccessor.HttpContext.Request.Query["controller"];
        //        var a = _httpContextAccessor.HttpContext.Request.Query["action"];
        //        var i = _httpContextAccessor.HttpContext.Request.Query["id"];
        //        var s = _httpContextAccessor.HttpContext.Request.Query["seourl"];
        //        if (!string.IsNullOrWhiteSpace(c))
        //        {
        //            return RedirectToAction(a, c, new { Id = i, seoUrl = s });
        //        }
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var findUser = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (findUser == null)
            {
                return RedirectToAction("Login");
            }
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(findUser, loginDTO.Password, false, false);
            if (result.Succeeded)
            {
                string cont = _httpContextAccessor.HttpContext.Request.Query["controller"].ToString();
                string act = _httpContextAccessor.HttpContext.Request.Query["action"].ToString();
                string id = _httpContextAccessor.HttpContext.Request.Query["id"].ToString();
                if (!String.IsNullOrWhiteSpace(cont))
                {
                    return RedirectToAction(act, cont, new { Id = id });
                }

                return RedirectToAction("Index", "Home");
            }
            return View(loginDTO);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var checkEmail = await _userManager.FindByEmailAsync(registerDto.Email);

            if (checkEmail != null)
            {
                ModelState.AddModelError("Error", "Email is exist!");
                return View(registerDto);
            }

            User newUser = new()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhotoUrl = "/"
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
                return View(registerDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

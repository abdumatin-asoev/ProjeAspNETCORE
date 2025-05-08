using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjeAspNETCORE.Models;

namespace ProjeAspNETCORE.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        // Constructor to initialize the database context, SignInManager, and UserManager
        public HomeController(Context context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Handles user login authentication
        [HttpPost]
        public async Task<IActionResult> CheckAuthentication(LoginViewModel model)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // If user not found, return to login page with error message
                ViewBag.ErrorMessage = "Invalid email or password!!";
                return View("Login");
            }

            // Authenticate the user and create a session
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                // Store user information in session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserRole", user.Role.ToString());

                // Redirect based on user role
                if (user.Role == UserRole.Admin)
                    return RedirectToAction("ListOfCarsForAdmin", "Admin");
                else
                    return RedirectToAction("ListOfCarsForUser", "User");
            }

            return View("Login");
        }

        // Displays the login page
        public IActionResult Login()
        {
            return View();
        }

        // Displays the registration page
        public IActionResult RegisterPage()
        {
            return View();
        }

        // Logs out the user and clears the session
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        // Handles user registration
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RegisterPage", model);
            }

            // Create a new user
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                Role = UserRole.Customer
            };

            // Save the user to the database
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                return RedirectToAction("Login", "Home");
            }

            // Handle errors during user creation
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("RegisterPage", model);
        }
    }
}


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using ProjeAspNETCORE.Models;
using System.Security.Claims;

namespace ProjeAspNETCORE.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public static User _user =new User();
        public HomeController(Context context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CheckAuthentication(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View("Index");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                _user = user;
                if (user.Role == UserRole.Admin)
                    return RedirectToAction("ListOfCarsForAdmin", "Home");
                else
                    return RedirectToAction("ListOfCarsForUser", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View("Index");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BackToLog() { 
            return RedirectToAction("Index"); 
        }
        public IActionResult RegisterPage() 
        {
            return View(); 
        }
        public IActionResult AdminHomePage() { 
            return View();
        }
        public IActionResult PostACar()
        {
            ViewBag.ListOfBrands = new SelectList(GetBrands());

            ViewBag.ListOfColors = new SelectList(GetColors());

            ViewBag.ListOfTypes = new SelectList(GetType());

            ViewBag.ListOfTransmissions = new SelectList(GetTRANSMISSIONS());
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PostACarControll(Car postACar, IFormFile ImagePath)
        {

            if (ImagePath != null && ImagePath.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImagePath.FileName);
                var extension = Path.GetExtension(ImagePath.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(stream);
                }
                
                postACar.ImagePath = "/uploads/" + newFileName;
            }
            Console.WriteLine(postACar.ImagePath);

           
            
            _context._Car.Add(postACar);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("PostACar");
        }
        public IActionResult Booking()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out long userId))
            {
                return Unauthorized();
            }
            
          //  List<BookACar> bookACars = _context._BookACar.ToList();
            List<BookACar> bookACars = _context._BookACar
                 .Include(b => b.User) 
                         .ToList();

            return View(bookACars);
        }
        public IActionResult AdminSearch() 

        {
            ViewBag.ListOfBrands = new SelectList(GetBrands());

            ViewBag.ListOfColors = new SelectList(GetColors());

            ViewBag.ListOfTypes = new SelectList(GetType());

            ViewBag.ListOfTransmissions = new SelectList(GetTRANSMISSIONS());
            SearchClass first = new SearchClass();
            return View(first);
        }
        public IActionResult LogOut()
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult CreateUser(User newuser, string password)
        {
            var userExist = _userManager.FindByEmailAsync(newuser.Email);   
            if (userExist.Result == null)
            {
                newuser.Role = UserRole.Customer;
                var result = _userManager.CreateAsync(newuser, password).Result;
				if (result.Succeeded)
				{
                   
					_userManager.AddToRoleAsync(newuser, "Customer").Wait();
					Console.WriteLine("✔ Customer создан успешно!");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						Console.WriteLine($"❌ Ошибка: {error.Description}");
					}
				}
			}
            return View("Index"); 
        }
        public IActionResult UserHomePage()
        {
            return View();
        }
        public IActionResult UserBookingPage()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out long userId))
            {
                return Unauthorized();
            }
            var bookings = _context._BookACar.Where(b => b.UserId==userId).ToList();

            return View(bookings); 
        }
        public IActionResult UserSearchPage()
        {
            ViewBag.ListOfBrands = new SelectList(GetBrands());

            ViewBag.ListOfColors = new SelectList(GetColors());

            ViewBag.ListOfTypes = new SelectList(GetType());

            ViewBag.ListOfTransmissions = new SelectList(GetTRANSMISSIONS());
            SearchClass first = new SearchClass();
            return View(first);
        }
        public IActionResult UserBookACar(long id)
        {
            Car car = _context._Car.Find(id);
            BookACar bookingCar = new BookACar();
            bookingCar.Car = car;
            bookingCar.User = _user;
            bookingCar.UserId = _user.Id;
            bookingCar.CarId = id;
            return View(bookingCar);
        }
        [HttpPost]
        public IActionResult Search(SearchClass search) 
        {
            var query = _context._Car.AsQueryable();

            if (!string.IsNullOrEmpty(search.Brand))
                query = query.Where(c => c.Brand == search.Brand);

            if (!string.IsNullOrEmpty(search.Type))
                query = query.Where(c => c.Type == search.Type);

            if (!string.IsNullOrEmpty(search.Color))
                query = query.Where(c => c.Color == search.Color);

            if (!string.IsNullOrEmpty(search.Transmission))
                query = query.Where(c => c.Transmission == search.Transmission);

            search.CarsFound = query.Select(c => new Car
            {
                Id = c.Id,
                Name = c.Name,
                Brand = c.Brand,
                Color = c.Color,
                Description = c.Description,
                ImagePath = c.ImagePath,
                Transmission = c.Transmission,
                Type = c.Type,
                Year = c.Year,
                Price = c.Price
            }).ToList();
            if (search.Id == "AdminSearch")
                return View("AdminSearch", search); 
            else
                return View("UserSearchPage",search); 
        }
        public IActionResult ListOfCarsForUser()
        {
            var cars = _context._Car.ToList();
            return View(cars);
        }
        public IActionResult ListOfCarsForAdmin()
        {
            var cars=_context._Car.ToList();
            return View(cars);
        }
        [HttpPost]
        public IActionResult Approve(long id)
        {
            var booking = _context._BookACar.Find(id);
            booking.BookCarStatus = BookCarStatus.Approved;
            _context.SaveChanges();
            return RedirectToAction("Booking");
        }
        [HttpPost]
        public IActionResult Reject(long id)
        {
            var booking = _context._BookACar.Find(id);
            booking.BookCarStatus = BookCarStatus.Rejected;
            _context.SaveChanges();
            return RedirectToAction("Booking");
        }
        public IActionResult DeleteACar(long id) 
        {
            var car = _context._Car.Find(id);
            if (car != null)
            {
                _context._Car.Remove(car);
                _context.SaveChanges();
            }

            return RedirectToAction("ListOfCarsForAdmin");
        }
        public IActionResult CarEditing(long id)
        {
            var car = _context._Car.Find(id);
            ViewBag.ListOfBrands = new SelectList(GetBrands());

            ViewBag.ListOfColors = new SelectList(GetColors());

            ViewBag.ListOfTypes = new SelectList(GetType());

            ViewBag.ListOfTransmissions = new SelectList(GetTRANSMISSIONS());
            return View(car); 
        }

        [HttpPost]
        public async Task<IActionResult> UpdateACar(Car updateCar, IFormFile ImagePath)
        {
            if (ImagePath != null && ImagePath.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImagePath.FileName);
                var extension = Path.GetExtension(ImagePath.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(stream);
                }

                updateCar.ImagePath = "/uploads/" + newFileName;
            }
            _context._Car.Update(updateCar);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListOfCarsForAdmin"); 
        }
        [HttpPost]
        public async Task<IActionResult> BookACar(BookACar booking) 
        {
            booking.Days = (booking.ToDate - booking.FromDate).Days;
            booking.Price = booking.Days * _context._Car.Find(booking.CarId).Price;
            _context._BookACar.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListOfCarsForUser"); 
        }
    
        private List<string> GetBrands()
        {
            return new List<string> {"Toyota", "BMW", "Honda","Mercedes","Bugatti","Nissan",
            "Mazda","Volkswagen","Audi","Ford","Porsche","Kia","Lamborghini", "Lexus" };
        }

        private List<string> GetColors()
        {
            return new List<string> {  
            "Black",
            "White",
            "Silver",
            "Gray",
            "Blue",
            "Red",
            "Green",
            "Brown",
            "Beige",
            "Yellow",
            "Orange",
            "Gold",
            "Burgundy",
            "Purple" };
        }

        private List<string> GetType()
        {
            return new List<string> { "Petrol", "Diesel", "Electric", "Hybrid" };
        }
        private List<string> GetTRANSMISSIONS()
        {
            return new List<string> { "Automatic", "Manual", "CVT", "Robot" };
        }

    }
    }

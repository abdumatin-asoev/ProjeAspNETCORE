using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjeAspNETCORE.Models;
using System.Security.Claims;

namespace ProjeAspNETCORE.Controllers
{
    public class AdminController : Controller
    {
        private readonly Context _context;

        public AdminController(Context context)
        {
            _context = context;
        }

        public IActionResult AdminHomePage()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        public IActionResult PostACar()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.ListOfBrands = new SelectList(CarProperties.GetBrands());

            ViewBag.ListOfColors = new SelectList(CarProperties.GetColors());

            ViewBag.ListOfTypes = new SelectList(CarProperties.GetType());

            ViewBag.ListOfTransmissions = new SelectList(CarProperties.GetTransmissions());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostACarControll(Car postACar, IFormFile ImagePath)
        {
            // Remove ImagePath from ModelState to prevent validation issues
            ModelState.Remove("ImagePath");

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns in case of validation errors
                ViewBag.ListOfBrands = new SelectList(CarProperties.GetBrands());
                ViewBag.ListOfColors = new SelectList(CarProperties.GetColors());
                ViewBag.ListOfTypes = new SelectList(CarProperties.GetType());
                ViewBag.ListOfTransmissions = new SelectList(CarProperties.GetTransmissions());
                return View("PostACar", postACar);
            }

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

            _context._Car.Add(postACar);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListOfCarsForAdmin");
        }

        public IActionResult AdminSearch()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.ListOfBrands = new SelectList(CarProperties.GetBrands());

            ViewBag.ListOfColors = new SelectList(CarProperties.GetColors());

            ViewBag.ListOfTypes = new SelectList(CarProperties.GetType());

            ViewBag.ListOfTransmissions = new SelectList(CarProperties.GetTransmissions());
            SearchClass first = new SearchClass();
            return View(first);
        }

        public IActionResult Booking()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out long userId))
            {
                return Unauthorized();
            }

            List<BookACar> bookACars = _context._BookACar
                 .Include(b => b.User)
                         .ToList();
            foreach (var booking in bookACars)
            {
                booking.Car = _context._Car.Find(booking.CarId);
            }

            return View(bookACars);
        }

        public IActionResult ListOfCarsForAdmin()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            var cars = _context._Car.ToList();
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
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            var car = _context._Car.Find(id);
            ViewBag.ListOfBrands = new SelectList(CarProperties.GetBrands());

            ViewBag.ListOfColors = new SelectList(CarProperties.GetColors());

            ViewBag.ListOfTypes = new SelectList(CarProperties.GetType());

            ViewBag.ListOfTransmissions = new SelectList(CarProperties.GetTransmissions());
            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateACar(Car updateCar, IFormFile ImagePath)
        {
            // Remove ImagePath from ModelState to prevent validation issues
            ModelState.Remove("ImagePath");

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns in case of validation errors
                ViewBag.ListOfBrands = new SelectList(CarProperties.GetBrands());
                ViewBag.ListOfColors = new SelectList(CarProperties.GetColors());
                ViewBag.ListOfTypes = new SelectList(CarProperties.GetType());
                ViewBag.ListOfTransmissions = new SelectList(CarProperties.GetTransmissions());
                return View("CarEditing", updateCar);
            }

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
    }
}

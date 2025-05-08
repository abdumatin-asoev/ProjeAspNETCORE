using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using ProjeAspNETCORE.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ProjeAspNETCORE.Controllers
{
    public class AdminController : Controller
    {
        private readonly Context _context;

        // Constructor to initialize the database context
        public AdminController(Context context)
        {
            _context = context;
        }

        // Displays the admin home page
        // This method checks if the admin is logged in by verifying the session.
        // If the session is invalid, the user is redirected to the login page.
        public IActionResult AdminHomePage()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // Displays the page to post a new car
        // This method populates dropdown lists for car properties (e.g., brands, colors, types, transmissions)
        // and renders the form for adding a new car.
        public IActionResult PostACar()
        {
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

        // Handles posting a new car
        // This method validates the car details submitted by the admin, processes the uploaded image,
        // and saves the car information to the database.
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

            // Process the uploaded image and save it to the server
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

            // Save the car details to the database
            _context._Car.Add(postACar);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListOfCarsForAdmin");
        }

        // Displays the admin search page
        // This method populates dropdown lists for search filters (e.g., brands, colors, types, transmissions)
        // and renders the search form for the admin.
        public IActionResult AdminSearch()
        {
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

        // Displays the booking management page
        // This method retrieves all bookings, including user and car details, and renders the booking management page.
        public IActionResult Booking()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out long userId))
            {
                return Unauthorized();
            }

            // Retrieve all bookings and include user and car details
            List<BookACar> bookACars = _context._BookACar
                 .Include(b => b.User)
                         .ToList();
            foreach (var booking in bookACars)
            {
                booking.Car = _context._Car.Find(booking.CarId);
            }

            return View(bookACars);
        }

        // Displays the list of cars for admin
        // This method retrieves all cars from the database and renders the car management page.
        public IActionResult ListOfCarsForAdmin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            var cars = _context._Car.ToList();
            return View(cars);
        }

        // Approves a booking
        // This method updates the status of a booking to "Approved" and saves the changes to the database.
        [HttpPost]
        public IActionResult Approve(long id)
        {
            var booking = _context._BookACar.Find(id);
            booking.BookCarStatus = BookCarStatus.Approved;
            _context.SaveChanges();
            return RedirectToAction("Booking");
        }

        // Rejects a booking
        // This method updates the status of a booking to "Rejected" and saves the changes to the database.
        [HttpPost]
        public IActionResult Reject(long id)
        {
            var booking = _context._BookACar.Find(id);
            booking.BookCarStatus = BookCarStatus.Rejected;
            _context.SaveChanges();
            return RedirectToAction("Booking");
        }

        // Deletes a car
        // This method removes a car from the database based on its ID.
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

        // Displays the car editing page
        // This method retrieves the details of a specific car and populates dropdown lists for editing.
        public IActionResult CarEditing(long id)
        {
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

        // Handles updating a car
        // This method validates the updated car details, processes the uploaded image (if any),
        // and saves the changes to the database.
        [HttpPost]
        public async Task<IActionResult> UpdateACar(Car updateCar, IFormFile ImagePath)
        {
            ModelState.Remove("ImagePath");

            if (!ModelState.IsValid)
            {
                ViewBag.ListOfBrands = new SelectList(CarProperties.GetBrands());
                ViewBag.ListOfColors = new SelectList(CarProperties.GetColors());
                ViewBag.ListOfTypes = new SelectList(CarProperties.GetType());
                ViewBag.ListOfTransmissions = new SelectList(CarProperties.GetTransmissions());
                return View("CarEditing", updateCar);
            }

            // Process the uploaded image and save it to the server
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

            // Save the updated car details to the database
            _context._Car.Update(updateCar);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListOfCarsForAdmin");
        }
    }
}

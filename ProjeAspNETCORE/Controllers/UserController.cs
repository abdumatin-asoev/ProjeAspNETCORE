using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using ProjeAspNETCORE.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ProjeAspNETCORE.Controllers
{
    public class UserController : Controller
    {
        private readonly Context _context;

        // Constructor to initialize the database context
        public UserController(Context context)
        {
            _context = context;
        }

        // Displays the user's home page
        public IActionResult UserHomePage()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // Displays the user's booking page
        public IActionResult UserBookingPage()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            // Retrieve bookings for the logged-in user
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out long userId))
            {
                return Unauthorized();
            }
            var bookings = _context._BookACar.Where(b => b.UserId == userId).ToList();
            foreach (var booking in bookings)
            {
                booking.Car = _context._Car.Find(booking.CarId);
            }

            return View(bookings);
        }

        // Displays the car booking page for a specific car
        public IActionResult UserBookACar(long id)
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
            User _user = _context._User.Find(userId);
            Car car = _context._Car.Find(id);
            BookACar bookingCar = new BookACar
            {
                Car = car,
                User = _user,
                UserId = _user.Id,
                CarId = id
            };
            return View(bookingCar);
        }

        // Displays the search page for users
        public IActionResult UserSearchPage()
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

        // Displays the list of cars available for users
        public IActionResult ListOfCarsForUser()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Home");
            }

            var cars = _context._Car.ToList();
            return View(cars);
        }

        // Handles car booking by the user
        [HttpPost]
        public async Task<IActionResult> BookACar(BookACar booking)
        {
            // Check for overlapping bookings
            var overlappingBookings = await _context._BookACar
                .Where(b => b.CarId == booking.CarId &&
                            b.ToDate >= booking.FromDate &&
                            b.FromDate <= booking.ToDate)
                .ToListAsync();

            if (overlappingBookings.Any())
            {
                ModelState.AddModelError("CarDateNotBookable", "The selected dates are unavailable for this car.");
                var car = _context._Car.Find(booking.CarId);
                var user = _context._User.Find(booking.UserId);
                booking.Car = car;
                booking.User = user;
                return View("UserBookACar", booking);
            }

            // Calculate days and price
            booking.Days = (booking.ToDate - booking.FromDate).Days;
            booking.Price = booking.Days * _context._Car.Find(booking.CarId).Price;

            // Save the booking
            _context._BookACar.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListOfCarsForUser");
        }
    }
}

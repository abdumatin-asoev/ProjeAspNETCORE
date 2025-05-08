using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using ProjeAspNETCORE.Models;

namespace ProjeAspNETCORE.Controllers
{
    public class SearchController : Controller
    {
        private readonly Context _context;

        // Constructor to initialize the database context
        public SearchController(Context context)
        {
            _context = context;
        }

        // Handles car search functionality
        [HttpPost]
        public IActionResult Search(SearchClass search)
        {
            // Repopulate dropdowns for search filters
            ViewBag.ListOfBrands = new SelectList(CarProperties.GetBrands());
            ViewBag.ListOfTypes = new SelectList(CarProperties.GetType());
            ViewBag.ListOfColors = new SelectList(CarProperties.GetColors());
            ViewBag.ListOfTransmissions = new SelectList(CarProperties.GetTransmissions());

            // Check if all filters are empty
            if (string.IsNullOrEmpty(search.Brand) &&
                string.IsNullOrEmpty(search.Type) &&
                string.IsNullOrEmpty(search.Color) &&
                string.IsNullOrEmpty(search.Transmission))
            {
                search.CarsFound = new List<Car>();
            }
            else
            {
                // Apply filters to the query
                var query = _context._Car.AsQueryable();

                if (!string.IsNullOrEmpty(search.Brand))
                    query = query.Where(c => c.Brand == search.Brand);

                if (!string.IsNullOrEmpty(search.Type))
                    query = query.Where(c => c.Type == search.Type);

                if (!string.IsNullOrEmpty(search.Color))
                    query = query.Where(c => c.Color == search.Color);

                if (!string.IsNullOrEmpty(search.Transmission))
                    query = query.Where(c => c.Transmission == search.Transmission);

                // Execute the query and populate the CarsFound list
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
            }

            // Return the appropriate view based on the search ID
            if (search.Id == "AdminSearch")
                return View("~/Views/Admin/AdminSearch.cshtml", search);
            else
                return View("~/Views/User/UserSearchPage.cshtml", search);
        }
    }
}

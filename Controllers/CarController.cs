using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;  // For IConfiguration
using NorbitsChallenge.Dal;              // For CarDb
using NorbitsChallenge.Models;           // For Car

namespace NorbitsChallenge.Controllers
{
    public class CarController : Controller
    {
        private readonly IConfiguration _config;

        public CarController(IConfiguration config)
        {
            _config = config;
        }

        // 0) Index: Just show a blank page or redirect to List
        public IActionResult Index()
        {
            // Example: auto-redirect to "List" for a default companyId of 1
            // return RedirectToAction("List", new { companyId = 1 });
            return View();
        }

        // 1) List all cars for a given companyId
        [HttpGet]
        public IActionResult List(int companyId)
        {
            var carDb = new CarDb(_config);
            var cars = carDb.GetCars(companyId);  // Use the DAL to fetch cars
            return View(cars);                   // Renders Views/Car/List.cshtml
        }

        // 2) Show details for one specific car
        [HttpGet]
        public IActionResult Details(int companyId, string licensePlate)
        {
            var carDb = new CarDb(_config);

            // Use the new GetCarByPlate method in CarDb
            var car = carDb.GetCarByPlate(companyId, licensePlate);
            if (car == null)
            {
                return NotFound(); // or redirect / show an error
            }
            return View(car); // Renders Views/Car/Details.cshtml
        }

        // 3) Create a new car - GET: Show form
        [HttpGet]
        public IActionResult Create(int companyId)
        {
            // Pre-set the CompanyId (optional if UI logic calls for it)
            var newCar = new Car { CompanyId = companyId };
            return View(newCar); // Renders Views/Car/Create.cshtml
        }

        // 3) Create a new car - POST: Form submission
        [HttpPost]
        public IActionResult Create(Car car)
        {
            var carDb = new CarDb(_config);
            carDb.CreateCar(car);

            // Redirect back to the list
            return RedirectToAction("List", new { companyId = car.CompanyId });
        }

        // 4) Delete car - GET: Confirm before deleting
        [HttpGet]
        public IActionResult Delete(int companyId, string licensePlate)
        {
            var carDb = new CarDb(_config);
            var car = carDb.GetCarByPlate(companyId, licensePlate);
            if (car == null)
            {
                return NotFound();
            }
            return View(car); // Renders Views/Car/Delete.cshtml
        }

        // 4) Delete car - POST: Actually delete after confirmation
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int companyId, string licensePlate)
        {
            var carDb = new CarDb(_config);
            carDb.DeleteCar(companyId, licensePlate);
            return RedirectToAction("List", new { companyId = companyId });
        }

        // 5) Edit car - GET: Show form with existing data
        [HttpGet]
        public IActionResult Edit(int companyId, string licensePlate)
        {
            var carDb = new CarDb(_config);
            var car = carDb.GetCarByPlate(companyId, licensePlate);
            if (car == null)
            {
                return NotFound();
            }
            return View(car); // Renders Views/Car/Edit.cshtml
        }

        // 5) Edit car - POST: Save changes to DB
        [HttpPost]
        public IActionResult Edit(Car car)
        {
            var carDb = new CarDb(_config);
            carDb.UpdateCar(car);
            return RedirectToAction("List", new { companyId = car.CompanyId });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class ServiceCityController : Controller
    {
        private AppDbContext db;

        public ServiceCityController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Index()
        {
            IEnumerable<ServiceCity> cities = db.ServiceCities.ToList();

            return View(cities);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create(ServiceCity city)
        {
            if (!ModelState.IsValid)
            {
                return View(city);
            }

            city.IsActive = true;

            db.ServiceCities.Add(city);
            db.SaveChanges();

            return RedirectToAction("Index","ServiceCity");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(int id)
        {
            ServiceCity city = db.ServiceCities.SingleOrDefault(x => x.Id == id);

            if (city == null)
                return HttpNotFound();
            
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(ServiceCity city)
        {
            if (!ModelState.IsValid)
            {
                return View(city);
            }

            ServiceCity cityInDb = db.ServiceCities.SingleOrDefault(x => x.Id == city.Id);

            if (city == null)
                return HttpNotFound();

            cityInDb.Name = city.Name;
            cityInDb.District = city.District;

            db.SaveChanges();

            return RedirectToAction("Index", "ServiceCity");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult ChangeActiveStatus(int id)
        {
            ServiceCity city = db.ServiceCities.SingleOrDefault(x => x.Id == id);

            if (city == null)
                return HttpNotFound();

            city.IsActive = !city.IsActive;

            db.SaveChanges();

            return RedirectToAction("index", "ServiceCity");
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using RapidWeb.Models;

namespace RapidWeb.Controllers
{
    public class PackageController : Controller
    {
        private AppDbContext db;

        public PackageController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.Activity)]
        public ActionResult Index()
        {
            IEnumerable<Package> packages = db.Packages.ToList();

            return View(packages);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create(Package package)
        {
            if (!ModelState.IsValid)
            {
                return View(package);
            }

            package.IsActive = true;

            db.Packages.Add(package);
            db.SaveChanges();

            return RedirectToAction("PackageDetail", new { id = package.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(int id)
        {
            Package package = db.Packages.SingleOrDefault(x => x.Id == id);

            if (package == null)
                return HttpNotFound();

            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(Package package)
        {
            if (!ModelState.IsValid)
            {
                return View(package);
            }

            Package packageInDb = db.Packages.SingleOrDefault(x => x.Id == package.Id);

            if (packageInDb == null)
                return HttpNotFound();

            packageInDb.Type = package.Type;
            packageInDb.Description = package.Description;
            packageInDb.Detail = package.Detail;
            packageInDb.Job = package.Job;
            packageInDb.CustomizeDetail = package.CustomizeDetail;
            packageInDb.ReferenceNote = package.ReferenceNote;
            packageInDb.Price = package.Price;

            db.SaveChanges();

            return RedirectToAction("PackageDetail", new { id = package.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult ChangeActiveStatus(int id)
        {
            Package package = db.Packages.SingleOrDefault(x => x.Id == id);

            if (package == null)
                return HttpNotFound();

            package.IsActive = !package.IsActive;

            db.SaveChanges();

            return RedirectToAction("index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.Activity)]
        public ActionResult PackageDetail(int id)
        {
            Package package = db.Packages.SingleOrDefault(x => x.Id == id);

            if (package == null)
                return HttpNotFound();

            return View(package);
        }
    }
}
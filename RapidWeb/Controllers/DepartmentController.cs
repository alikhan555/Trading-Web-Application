using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class DepartmentController : Controller
    {
        private AppDbContext db;

        public DepartmentController()
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
            IEnumerable<Department> departments = db.Departments.ToList();

            return View(departments);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            department.IsActive = true;

            db.Departments.Add(department);
            db.SaveChanges();

            return RedirectToAction("Index","Department");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(int id)
        {
            Department department = db.Departments.SingleOrDefault(x => x.Id == id);

            if (department == null)
                return HttpNotFound();

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            Department departmentInDb = db.Departments.SingleOrDefault(x => x.Id == department.Id);

            if (departmentInDb == null)
                return HttpNotFound();

            departmentInDb.Name = department.Name;
            db.SaveChanges();

            return RedirectToAction("Index", "Department");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult ChangeActiveStatus(int id)
        {
            Department department = db.Departments.SingleOrDefault(x => x.Id == id);

            if (department == null)
                return HttpNotFound();

            department.IsActive = !department.IsActive;

            db.SaveChanges();

            return RedirectToAction("index", "Department");
        }
    }
}
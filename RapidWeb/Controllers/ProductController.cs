using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RapidWeb.Models;

namespace RapidWeb.Controllers
{
    public class ProductController : Controller
    {
        private AppDbContext db;

        public ProductController()
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
            IEnumerable<Product> products = db.Products.ToList();

            return View(products);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            product.Quantity = 0;
            product.IsActive = true;

            db.Products.Add(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(int id)
        {
            Product product = db.Products.SingleOrDefault(x => x.Id == id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            Product productInDb = db.Products.SingleOrDefault(x => x.Id == product.Id);

            if (productInDb == null)
                return HttpNotFound();

            productInDb.Description = product.Description;
            productInDb.SerialNo = product.SerialNo;
            productInDb.CreationDate = product.CreationDate;
            productInDb.Cost = product.Cost;
            productInDb.SampleQuantity = product.SampleQuantity;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult ChangeActiveStatus(int id)
        {
            Product product = db.Products.SingleOrDefault(x => x.Id == id);

            if (product == null)
                return HttpNotFound();

            product.IsActive = !product.IsActive;
            
            db.SaveChanges();

            return RedirectToAction("index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.Inventory)]
        public ActionResult GetProductAllProducts()
        {
            var products = db.Products
                .Where(x => x.IsActive)
                .ToList()
                .Select(x => new 
                {
                    x.Id,
                    x.Cost,
                    x.Quantity,
                    x.Description,
                });

            return Json(products,JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using RapidWeb.Models;

namespace RapidWeb.Controllers
{
    public class VendorController : Controller
    {
        private AppDbContext db;

        public VendorController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.Activity)]
        public ActionResult Index()
        {
            IEnumerable<Vendor> vendors = db.Vendors.ToList();

            return View(vendors);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult Create(Vendor vendor)
        {
            if (!ModelState.IsValid)
            {
                return View(vendor);
            }

            vendor.IsActive = !vendor.IsActive;

            db.Vendors.Add(vendor);
            db.SaveChanges();

            return RedirectToAction("VendorDetail", new { id = vendor.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult Edit(int id)
        {
            Vendor vendor = db.Vendors.SingleOrDefault(x => x.Id == id);

            if (vendor == null)
                return HttpNotFound();

            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult Edit(Vendor vendor)
        {
            if (!ModelState.IsValid)
            {
                return View(vendor);
            }

            Vendor vendorInDb = db.Vendors.SingleOrDefault(x => x.Id == vendor.Id);

            if (vendorInDb == null)
                return HttpNotFound();

            vendorInDb.Name = vendor.Name;
            vendorInDb.City = vendor.City;
            vendorInDb.PaymentType = vendor.PaymentType;
            vendorInDb.NTNNo = vendor.NTNNo;
            vendorInDb.Contact = vendor.Contact;
            vendorInDb.Address = vendor.Address;
            vendorInDb.Email = vendor.Email;

            db.SaveChanges();

            return RedirectToAction("VendorDetail", new { id = vendor.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult ChangeActiveStatus(int id)
        {
            Vendor vendor = db.Vendors.SingleOrDefault(x => x.Id == id);

            if (vendor == null)
                return HttpNotFound();

            vendor.IsActive = !vendor.IsActive;
            
            db.SaveChanges();

            return RedirectToAction("index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.Activity)]
        public ActionResult VendorDetail(int id)
        {
            Vendor vendor = db.Vendors.SingleOrDefault(x => x.Id == id);

            if (vendor == null)
                return HttpNotFound();

            return View(vendor);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.Activity)]
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.Activity)]
        public ActionResult GetVendors(int vendorIdFrom = -1, int vendorIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1)      // CityNotUsed b/c no relation (manual)
        {
            IQueryable<Vendor> vendorsQry = db.Vendors;

            if (vendorIdFrom != -1)
                vendorsQry = vendorsQry.Where(x => x.Id >= vendorIdFrom);
            if (vendorIdTo != -1)
                vendorsQry = vendorsQry.Where(x => x.Id <= vendorIdTo);

            List<Vendor> vendors = vendorsQry.ToList();

            return Json(vendors, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.Activity)]
        public ActionResult VendorsReport(int vendorIdFrom = -1, int vendorIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1)
        {
            IQueryable<Vendor> vendorsQry = db.Vendors;

            if (vendorIdFrom != -1)
                vendorsQry = vendorsQry.Where(x => x.Id >= vendorIdFrom);
            if (vendorIdTo != -1)
                vendorsQry = vendorsQry.Where(x => x.Id <= vendorIdTo);

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Vendor/VendorsDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(vendorsQry.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("VendorsList_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.Activity)]
        public ActionResult VendorIndividualReport(int id)
        {
            Vendor vendor = db.Vendors.SingleOrDefault(x => x.Id == id);

            if (vendor == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Vendor/VendorIndividualReport.rpt")));

            report.Database.Tables[0].SetDataSource(new List<Vendor> { vendor });

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("VendorIndividual_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using RapidWeb.Models;

namespace RapidWeb.Controllers
{
    public class ServiceController : Controller
    {
        private AppDbContext db;

        public ServiceController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.RTPUser, Role.Activity)]
        public ActionResult Index()
        {
            IEnumerable<Service> services = db.Services.ToList();

            return View(services);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Create(Service service)
        {
            if (!ModelState.IsValid)
            {
                return View(service);
            }

            if (!service.Development && !service.Installation && !service.Maintenance && !service.Networking && !service.Troubleshooting && !service.Other)
            {
                ModelState.AddModelError(String.Empty, "At least one Service Nature must be selected.");

                return View(service);
            }
            
            service.IsActive = true;

            db.Services.Add(service);
            db.SaveChanges();

            return RedirectToAction("ServiceDetail", new {id = service.Id});
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(int id)
        {
            Service service = db.Services.SingleOrDefault(x => x.Id == id);

            if (service == null)
                return HttpNotFound();

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult Edit(Service service)
        {
            if (!ModelState.IsValid)
            {
                return View(service);
            }

            if (!service.Development && !service.Installation && !service.Maintenance && !service.Networking && !service.Troubleshooting && !service.Other)
            {
                ModelState.AddModelError(String.Empty, "At least one Service Nature must be selected.");

                return View(service);
            }

            Service serviceInDb = db.Services.SingleOrDefault(x => x.Id == service.Id);

            if (serviceInDb == null)
                return HttpNotFound();

            serviceInDb.Description = service.Description;
            serviceInDb.PackageType = service.PackageType;
            serviceInDb.Maintenance = service.Maintenance;
            serviceInDb.Installation = service.Installation;
            serviceInDb.Development = service.Development;
            serviceInDb.Troubleshooting = service.Troubleshooting;
            serviceInDb.Networking = service.Networking;
            serviceInDb.Other = service.Other;
            serviceInDb.PackageDetails = service.PackageDetails;

            db.SaveChanges();

            return RedirectToAction("ServiceDetail", new { id = service.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public ActionResult ChangeActiveStatus(int id)
        {
            Service service = db.Services.SingleOrDefault(x => x.Id == id);

            if (service == null)
                return HttpNotFound();

            service.IsActive = !service.IsActive;

            db.SaveChanges();

            return RedirectToAction("index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.RTPUser, Role.Activity)]
        public ActionResult ServiceDetail(int id)
        {
            Service service = db.Services.SingleOrDefault(x => x.Id == id);

            if (service == null)
                return HttpNotFound();

            return View(service);
        }
        
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.RTPUser, Role.Activity)]
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.RTPUser, Role.Activity)]
        public ActionResult GetServices(int serviceIdFrom = -1, int serviceIdTo = -1)
        {
            IQueryable<Service> servicesQry = db.Services;

            if (serviceIdFrom != -1)
                servicesQry = servicesQry.Where(x => x.Id >= serviceIdFrom);
            if (serviceIdTo != -1)
                servicesQry = servicesQry.Where(x => x.Id <= serviceIdTo);
            
            List<Service> services = servicesQry.ToList();

            return Json(services, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.RTPUser, Role.Activity)]
        public ActionResult ServicesReport(int serviceIdFrom = -1, int serviceIdTo = -1)
        {
            IQueryable<Service> servicesQry = db.Services;

            if (serviceIdFrom != -1)
                servicesQry = servicesQry.Where(x => x.Id >= serviceIdFrom);
            if (serviceIdTo != -1)
                servicesQry = servicesQry.Where(x => x.Id <= serviceIdTo);

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Service/ServicesDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(servicesQry.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("ServicesList_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
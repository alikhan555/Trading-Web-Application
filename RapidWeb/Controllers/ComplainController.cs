using CrystalDecisions.CrystalReports.Engine;
using RapidWeb.Models;
using RapidWeb.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapidWeb.Controllers
{
    public class ComplainController : Controller
    {
        private AppDbContext db;

        public ComplainController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult Index()
        {
            IEnumerable<Complain> complains = db.Complains
                .Include(x => x.Client.City)
                .Include(x => x.Employee).ToList();

            return View(complains);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult NewComplain()
        {
            return View(db.Clients.Where(x => x.IsActive).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult ComplainForm(int id)                    
        {
            Client client = db.Clients
                .Where(x => x.IsActive)
                .Include(x => x.City)
                .SingleOrDefault(x => x.Id == id);

            if (client == null)
                return HttpNotFound();

            ComplainViewModel complainViewModel = new ComplainViewModel()
            {
                Complain = new Complain()
                {
                    ContactPerson = client.Name,
                    ClientId = client.Id,
                },
                Client = client,
                Employee = db.Employees.Where(x => x.IsActive).ToList(),
                Service = db.Services.Where(x => x.IsActive).ToList()
            };

            return View(complainViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult CreateComplain(ComplainViewModel complainViewModel)
        {
            if (complainViewModel.Complain == null || !db.Clients.Any(x => x.Id == complainViewModel.Complain.ClientId))
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                Client client = db.Clients.Where(x => x.IsActive)
                    .Include(x => x.City)
                    .SingleOrDefault(x => x.Id == complainViewModel.Complain.ClientId);

                if (client == null)
                    return HttpNotFound();

                complainViewModel.Client = client;
                complainViewModel.Employee = db.Employees.Where(x => x.IsActive).ToList();
                complainViewModel.Service = db.Services.Where(x => x.IsActive).ToList();

                return View("ComplainForm", complainViewModel);
            }

            complainViewModel.Complain.Status = false;
            complainViewModel.Complain.CreationDateTime = DateTime.Now;
            complainViewModel.Complain.CompletionDateTime = DateTime.Now;
            db.Complains.Add(complainViewModel.Complain);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult ComplainAssignment(int id)      // id == ComplainId
        {
            Complain complain = db.Complains
                .Include(x => x.Client)
                .Include(x => x.Employee)
                .SingleOrDefault(x => x.Id == id);

            if (complain == null)
                return HttpNotFound();

            if (complain.Status)
                throw new HttpException(400, "Assignment has already completed for complain: " + complain.PrefixId);

            return View(complain);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult ComplainAssignment(Complain complain)
        {
            Complain complainInDb = db.Complains.SingleOrDefault(x => x.Id == complain.Id);

            if (complainInDb == null)
                throw new HttpException(404,"Complain Not Found");

            if (complain.AssignmentProducts != null && complain.AssignmentProducts.Any(x => x.ProductId == 0 || x.Qty < 1 || x.Cost < 1))
                throw new HttpException(400, "Bad Request. Select a valid product, Quantity, Cost");

            complainInDb.Status = true;
            complainInDb.CompletionDateTime = DateTime.Now;
            complainInDb.ProgressReport = complain.ProgressReport;
            complainInDb.AssignmentProducts = complain.AssignmentProducts;
            db.SaveChanges();

            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult Detail(int id)
        {
            Complain complain = db.Complains.SingleOrDefault(x => x.Id == id);

            if (complain == null)
                return HttpNotFound();

            switch (complain.Status)
            {
                case true:
                {
                    return RedirectToAction("AssignmentDetail", new { id = id });
                    }
                case false:
                {
                    return RedirectToAction("ComplainDetail", new {id = id});
                }
                default:
                {
                    return HttpNotFound();
                }
            }
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult ComplainDetail(int id)
        {
            Complain complain = db.Complains
                .Include(x => x.Client.City)
                .Include(x => x.Employee)
                .SingleOrDefault(x => x.Id == id);

            if (complain == null)
                return HttpNotFound();

            return View(complain);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult AssignmentDetail(int id)
        {
            Complain complain = db.Complains
                .Include(x => x.Employee)
                .Include(x => x.Client)
                .SingleOrDefault(x => x.Id == id);

            if (complain == null)
                return HttpNotFound();

            complain.AssignmentProducts = db.ComplainProductsDetails
                .Include(x => x.Product)
                .Where(x => x.ComplainId == complain.Id)
                .ToList();

            return View(complain);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult GetComplains(int customerIdFrom = -1, int customerIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1, DateTime? dateFrom = null, DateTime? dateTo = null, bool? status = null)
        {
            IQueryable<Complain> complainsQry = db.Complains;

            if (customerIdFrom != -1)
                complainsQry = complainsQry.Where(x => x.ClientId >= customerIdFrom);
            if (customerIdTo != -1)
                complainsQry = complainsQry.Where(x => x.ClientId <= customerIdTo);

            if (cityIdFrom != -1)
                complainsQry = complainsQry.Where(x => x.Client.CityId >= cityIdFrom);
            if (cityIdTo != -1)
                complainsQry = complainsQry.Where(x => x.Client.CityId <= cityIdTo);

            if (dateFrom != null)
                complainsQry = complainsQry.Where(x => x.CreationDateTime >= dateFrom);
            if (dateTo != null)
                complainsQry = complainsQry.Where(x => x.CreationDateTime <= dateTo);

            if (status != null)
                complainsQry = complainsQry.Where(x => x.Status == status);

            List<Complain> employees = complainsQry
                .Include(x => x.Client.City)
                .Include(x => x.Employee)
                .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult ComplainsReport(int customerIdFrom = -1, int customerIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1, DateTime? dateFrom = null, DateTime? dateTo = null, bool? status = null)
        {
            IQueryable<Complain> complainsQry = db.Complains;

            if (customerIdFrom != -1)
                complainsQry = complainsQry.Where(x => x.ClientId >= customerIdFrom);
            if (customerIdTo != -1)
                complainsQry = complainsQry.Where(x => x.ClientId <= customerIdTo);

            if (cityIdFrom != -1)
                complainsQry = complainsQry.Where(x => x.Client.CityId >= cityIdFrom);
            if (cityIdTo != -1)
                complainsQry = complainsQry.Where(x => x.Client.CityId <= cityIdTo);

            if (dateFrom != null)
                complainsQry = complainsQry.Where(x => x.CreationDateTime >= dateFrom);
            if (dateTo != null)
                complainsQry = complainsQry.Where(x => x.CreationDateTime <= dateTo);

            if (status != null)
                complainsQry = complainsQry.Where(x => x.Status == status);

            var complains = complainsQry.Include(x => x.Client).Include(x => x.Employee).ToList();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Complain/ComplainsDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(complains);
            report.Database.Tables[1].SetDataSource(db.Clients.ToList());
            report.Database.Tables[2].SetDataSource(db.Employees.ToList());
            report.Database.Tables[3].SetDataSource(db.ServiceCities.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("ComplainsList_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult ComplainIndividualReport(int id)
        {
            Complain complain = db.Complains.SingleOrDefault(x => x.Id == id);

            if (complain == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Complain/ComplainIndividualReport.rpt")));

            report.Database.Tables[0].SetDataSource(new List<Complain> { complain });
            report.Database.Tables[1].SetDataSource(db.Clients.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("ClientsIndividual_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
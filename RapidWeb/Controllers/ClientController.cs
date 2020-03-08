using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Ajax.Utilities;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class ClientController : Controller
    {
        private AppDbContext db;

        public ClientController()
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
            IEnumerable<Client> clients = db.Clients.Include(x => x.City).Include(x => x.Package).ToList();

            return View(clients);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult Create()
        {
            ClientViewModel clientViewModel = new ClientViewModel()
            {
                Client = new Client(),
                ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList(),
                Packages = db.Packages.Where(x => x.IsActive).ToList()
            };
            
            return View(clientViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult Create(Client client)
        {
            if (!ModelState.IsValid)
            {
                ClientViewModel clientViewModel = new ClientViewModel()
                {
                    Client = client,
                    ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList(),
                    Packages = db.Packages.Where(x => x.IsActive).ToList()
                };

                return View(clientViewModel);
            }

            client.IsActive = true;

            db.Clients.Add(client);
            db.SaveChanges();

            return RedirectToAction("ClientDetail", new { id = client.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult Edit(int id)
        {
            Client client = db.Clients.SingleOrDefault(x => x.Id == id);

            if (client == null)
                return HttpNotFound();

            ClientViewModel clientViewModel = new ClientViewModel()
            {
                Client = client,
                ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList(),
                Packages = db.Packages.Where(x => x.IsActive).ToList()
            };

            return View(clientViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult Edit(Client client)
        {
            if (!ModelState.IsValid)
            {
                ClientViewModel clientViewModel = new ClientViewModel()
                {
                    Client = client,
                    ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList(),
                    Packages = db.Packages.Where(x => x.IsActive).ToList()
                };

                return View(clientViewModel);
            }

            Client clientInDb = db.Clients.SingleOrDefault(x => x.Id == client.Id);

            if (clientInDb == null)
                return HttpNotFound();

            clientInDb.RefCode = client.RefCode;
            clientInDb.Name = client.Name;
            clientInDb.CityId = client.CityId;
            clientInDb.Address = client.Address;
            clientInDb.Area = client.Area;
            clientInDb.ContactPersonName = client.ContactPersonName;
            clientInDb.ContactNo = client.ContactNo;
            clientInDb.ContactLandLine = client.ContactLandLine;
            clientInDb.PackageId = client.PackageId;
            clientInDb.Email = client.Email;
            clientInDb.OnDays = client.OnDays;
            clientInDb.Timing = client.Timing;
            clientInDb.CreditDays = client.CreditDays;

            db.SaveChanges();

            return RedirectToAction("ClientDetail", new { id = client.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator)]
        public ActionResult ChangeActiveStatus(int id)
        {
            Client client = db.Clients.SingleOrDefault(x => x.Id == id);

            if (client == null)
                return HttpNotFound();

            client.IsActive = !client.IsActive;

            db.SaveChanges();

            return RedirectToAction("index");
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult ClientDetail(int id)
        {
            Client client = db.Clients
                .Include(x => x.City)
                .Include(x => x.Package)
                .SingleOrDefault(x => x.Id == id);

            if (client == null)
                return HttpNotFound();

            return View(client);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult GetClients(int clientIdFrom = -1, int clientIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1)
        {
            IQueryable<Client> clientsQry = db.Clients;

            if (clientIdFrom != -1)
                clientsQry = clientsQry.Where(x => x.Id >= clientIdFrom);
            if (clientIdTo != -1)
                clientsQry = clientsQry.Where(x => x.Id <= clientIdTo);

            if (cityIdFrom != -1)
                clientsQry = clientsQry.Where(x => x.CityId >= cityIdFrom);
            if (cityIdTo != -1)
                clientsQry = clientsQry.Where(x => x.CityId <= cityIdTo);
            
            List<Client> clients = clientsQry.Include(x => x.City).Include(x => x.Package).ToList();

            return Json(clients, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult ClientDetailReport(int clientIdFrom = -1, int clientIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1)
        {
            IQueryable<Client> employeesQry = db.Clients;

            if (clientIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.Id >= clientIdFrom);
            if (clientIdTo != -1)
                employeesQry = employeesQry.Where(x => x.Id <= clientIdTo);

            if (cityIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.CityId >= cityIdFrom);
            if (cityIdTo != -1)
                employeesQry = employeesQry.Where(x => x.CityId <= cityIdTo);

            List<Client> employees = employeesQry.Include(x => x.City).ToList();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Client/ClientDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(employees);
            report.Database.Tables[1].SetDataSource(db.ServiceCities.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("ClientDetailReport_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.DataOperator, Role.RTPUser, Role.Activity)]
        public ActionResult ClientIndividualReport(int id)
        {
            Client client = db.Clients.SingleOrDefault(x => x.Id == id);

            if (client == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Client/ClientIndividualReport.rpt")));

            report.Database.Tables[0].SetDataSource(new List<Client> { client });
            report.Database.Tables[1].SetDataSource(db.ServiceCities.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("ClientIndividualReport_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
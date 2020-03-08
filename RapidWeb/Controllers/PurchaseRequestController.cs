using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using CrystalDecisions.CrystalReports.Engine;
using RapidWeb.Models;

namespace RapidWeb.Controllers
{
    public class PurchaseRequestController : Controller
    {
        private AppDbContext db;

        public PurchaseRequestController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult Index()
        {
            IEnumerable<PurchaseRequest> purchaseRequests = db.PurchaseRequests.ToList().OrderBy(x => x.Id);

            return View(purchaseRequests);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult PurchaseRequestForm()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult PurchaseRequestForm(PurchaseRequest purchaseRequest)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(400,"Bad Request. Fill all fields Correctly");
            }

            if (purchaseRequest.PurchaseRequestProducts.Count < 1 || purchaseRequest.PurchaseRequestProducts.Any(x => x.ProductId == 0 || x.RequiredQty < 1 || x.UnitPrice < 1))
                throw new HttpException(400, "Bad Request. Select a valid product, Quantity, Cost and at least one product in request");

            foreach (PurchaseRequestProduct product in purchaseRequest.PurchaseRequestProducts)
                product.RemainingQty = product.RequiredQty;
            
            purchaseRequest.CreationDateTime = DateTime.Now;
            db.PurchaseRequests.Add(purchaseRequest);
            db.SaveChanges();

            return Json(purchaseRequest.Id, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult GetPurchaseRequests(int purchaseRequestIdFrom = -1, int purchaseRequestIdTo = -1, DateTime? purchaseRequestDateFrom = null, DateTime? purchaseRequestDateTo = null)
        {
            IQueryable<PurchaseRequest> purchaseRequestsQry = db.PurchaseRequests;

            if (purchaseRequestIdFrom != -1)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.Id >= purchaseRequestIdFrom);
            if (purchaseRequestIdTo != -1)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.Id <= purchaseRequestIdTo);
            
            if (purchaseRequestDateFrom != null)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.CreationDateTime >= purchaseRequestDateFrom);
            if (purchaseRequestDateTo != null)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.CreationDateTime <= purchaseRequestDateTo);
            
            List<PurchaseRequest> purchaseRequests = purchaseRequestsQry.ToList();

            return Json(purchaseRequests, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult PurchaseRequestsReport(int purchaseRequestIdFrom = -1, int purchaseRequestIdTo = -1, DateTime? purchaseRequestDateFrom = null, DateTime? purchaseRequestDateTo = null)
        {
            IQueryable<PurchaseRequest> purchaseRequestsQry = db.PurchaseRequests;

            if (purchaseRequestIdFrom != -1)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.Id >= purchaseRequestIdFrom);
            if (purchaseRequestIdTo != -1)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.Id <= purchaseRequestIdTo);

            if (purchaseRequestDateFrom != null)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.CreationDateTime >= purchaseRequestDateFrom);
            if (purchaseRequestDateTo != null)
                purchaseRequestsQry = purchaseRequestsQry.Where(x => x.CreationDateTime <= purchaseRequestDateTo);

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/PurchaseRequest/PurchaseRequestsDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(purchaseRequestsQry.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("PurchaseRequestsList_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult PurchaseRequestDetail(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequests.SingleOrDefault(x => x.Id == id);

            if (purchaseRequest == null)
                return HttpNotFound();

            purchaseRequest.PurchaseRequestProducts = db.PurchaseRequestProducts
                .Include(x => x.Product)
                .Where(x => x.PurchaseRequestId == id)
                .ToList();

            return View(purchaseRequest);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult PurchaseRequestPdf(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequests
                .Include(x => x.PurchaseRequestProducts)
                .SingleOrDefault(x => x.Id == id);

            if (purchaseRequest == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/PurchaseRequest/PurchaseRequestIndividualReport.rpt")));

            report.Database.Tables[0].SetDataSource(new List<PurchaseRequest> { purchaseRequest });
            report.Database.Tables[1].SetDataSource(purchaseRequest.PurchaseRequestProducts);
            report.Database.Tables[2].SetDataSource(db.Products.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("PurchaseRequestIndividual_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult GetPurchaseRequestProducts(int id)          //id = purchase request
        {
            List<PurchaseRequestProduct> purchaseRequestProducts = db.PurchaseRequestProducts
                .Where(x => x.PurchaseRequestId == id && x.RemainingQty > 0)
                .Include(x=> x.Product)
                .ToList();

            return Json(purchaseRequestProducts, JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private AppDbContext db;

        public PurchaseOrderController()
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
            IEnumerable<PurchaseOrder> purchaseRequests = db.PurchaseOrders
                .Include(x => x.Vendor)
                .Include(x => x.PurchaseRequest)
                .Include(x => x.PurchaseOrderProducts)
                .ToList();

            return View(purchaseRequests);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult PurchaseOrderForm()
        {
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel()
            {
                Vendors = db.Vendors.Where(x => x.IsActive).ToList(),
                PurchaseRequests = db.PurchaseRequests
            };

            return View(purchaseOrderViewModel);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult PurchaseOrderForm(PurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Bad Request. Fill all fields Correctly");
            }

            if(purchaseOrder.VendorId == 0 || purchaseOrder.PurchaseRequestId == 0)
                throw new HttpException(400, "Bad Request. PurchaseRequestId and VendorId is required.");

            if (purchaseOrder.PurchaseOrderProducts == null || purchaseOrder.PurchaseOrderProducts.Any(x => x.ProductId == 0 || x.RequiredQty < 0 || x.UnitPrice < 1))
                throw new HttpException(400, "Bad Request. Select a valid product, Quantity and Cost.");

            purchaseOrder.PurchaseOrderProducts = purchaseOrder
                .PurchaseOrderProducts.Where(x => x.RequiredQty > 0).ToList();

            PurchaseRequest purchaseRequest = db.PurchaseRequests
                .Include(x => x.PurchaseRequestProducts)
                .SingleOrDefault(x => x.Id == purchaseOrder.PurchaseRequestId);

            if (purchaseRequest == null)
                HttpNotFound();

            foreach (PurchaseOrderProduct POProduct in purchaseOrder.PurchaseOrderProducts)
            {
                PurchaseRequestProduct PRP = purchaseRequest.PurchaseRequestProducts.SingleOrDefault(x => x.ProductId == POProduct.ProductId);
                if (PRP == null) return HttpNotFound();

                if (PRP.RemainingQty < POProduct.RequiredQty)
                    throw new HttpException(400, "RequiredQty must not be exceed RemainingQty.");

                PRP.RemainingQty -= POProduct.RequiredQty;

                POProduct.RemainingQty = POProduct.RequiredQty;
            }

            purchaseOrder.CreationDateTime = DateTime.Now;
            db.PurchaseOrders.Add(purchaseOrder);
            db.SaveChanges();

            return Json(purchaseOrder.Id, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult GetPurchaseOrders(int purchaseOrderIdFrom = -1, int purchaseOrderIdTo = -1, int vendorIdFrom = -1, int vendorIdTo = -1, DateTime? purchaseOrderDateFrom = null, DateTime? purchaseOrderDateTo = null)
        {
            IQueryable<PurchaseOrder> purchaseOrdersQry = db.PurchaseOrders;

            if (purchaseOrderIdFrom != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.Id >= purchaseOrderIdFrom);
            if (purchaseOrderIdTo != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.Id <= purchaseOrderIdTo);

            if (vendorIdFrom != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.VendorId >= vendorIdFrom);
            if (vendorIdTo != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.VendorId <= vendorIdTo);

            if (purchaseOrderDateFrom != null)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.CreationDateTime >= purchaseOrderDateFrom);
            if (purchaseOrderDateTo != null)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.CreationDateTime <= purchaseOrderDateTo);

            var purchaseRequests = purchaseOrdersQry
                .Include(x => x.PurchaseOrderProducts)
                .Include(x => x.Vendor)
                .Select(x => new 
                {
                    x.Id,
                    PrefixId = PurchaseOrder.Prefix + x.Id,
                    x.CreationDateTime,
                    x.PurchaseRequestId,
                    PurchaseRequestPrefixId = PurchaseRequest.Prefix + x.PurchaseRequestId,
                    VendorPrefixId = Vendor.Prefix + x.Vendor.Id,
                    VendorName = x.Vendor.Name,
                    TotalAmount =   x.PurchaseOrderProducts.Count == 0 ? 0 : x.PurchaseOrderProducts.Select(y => new { x = y.UnitPrice * y.RequiredQty }).Sum(z => z.x)    
                })
                .ToList();

            return Json(purchaseRequests, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult PurchaseOrdersReport(int purchaseOrderIdFrom = -1, int purchaseOrderIdTo = -1, int vendorIdFrom = -1, int vendorIdTo = -1, DateTime? purchaseOrderDateFrom = null, DateTime? purchaseOrderDateTo = null)
        {
            IQueryable<PurchaseOrder> purchaseOrdersQry = db.PurchaseOrders;

            if (purchaseOrderIdFrom != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.Id >= purchaseOrderIdFrom);
            if (purchaseOrderIdTo != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.Id <= purchaseOrderIdTo);

            if (vendorIdFrom != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.VendorId >= vendorIdFrom);
            if (vendorIdTo != -1)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.VendorId <= vendorIdTo);

            if (purchaseOrderDateFrom != null)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.CreationDateTime >= purchaseOrderDateFrom);
            if (purchaseOrderDateTo != null)
                purchaseOrdersQry = purchaseOrdersQry.Where(x => x.CreationDateTime <= purchaseOrderDateTo);

            var list = purchaseOrdersQry.Include(x => x.PurchaseOrderProducts).ToList();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/PurchaseOrder/PurchaseOrdersDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(list);
            report.Database.Tables[1].SetDataSource(db.PurchaseRequests.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("PurchaseOrdersList_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult PurchaseOrderDetail(int id)
        {
            PurchaseOrder purchaseOrder = db.PurchaseOrders
                .Include(x => x.Vendor)
                .SingleOrDefault(x => x.Id == id);

            if (purchaseOrder == null)
                return HttpNotFound();

            purchaseOrder.PurchaseOrderProducts = db.PurchaseOrderProducts
                .Include(x => x.Product)
                .Where(x => x.PurchaseOrderId == id)
                .ToList();

            return View(purchaseOrder);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser, Role.Activity)]
        public ActionResult PurchaseOrderPdf(int id)
        {
            PurchaseOrder purchaseOrder = db.PurchaseOrders
                .Include(x => x.PurchaseOrderProducts)
                .SingleOrDefault(x => x.Id == id);

            if (purchaseOrder == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/PurchaseOrder/PurchaseOrderIndividualReport.rpt")));

            report.Database.Tables[0].SetDataSource(new List<PurchaseOrder> { purchaseOrder });
            report.Database.Tables[1].SetDataSource(purchaseOrder.PurchaseOrderProducts);
            report.Database.Tables[2].SetDataSource(db.Products.ToList());
            report.Database.Tables[3].SetDataSource(db.Vendors.Where(x => x.Id == purchaseOrder.VendorId).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("PurchaseOrderIndividual_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult GetPurchaseOrderProducts(int id)            // id = purchase order Id
        {
            List<PurchaseOrderProduct> purchaseOrderProducts = db.PurchaseOrderProducts
                .Where(x => x.PurchaseOrderId == id && x.RemainingQty > 0)
                .Include(x => x.Product)
                .ToList();

            return Json(purchaseOrderProducts, JsonRequestBehavior.AllowGet);
        }
    }
}
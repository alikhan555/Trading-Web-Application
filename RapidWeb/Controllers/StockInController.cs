using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Ajax.Utilities;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class StockInController : Controller
    {
        private AppDbContext db;

        public StockInController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult Index()
        {
            IEnumerable<StockIn> stockIns = db.StockIns
                .Include(x => x.PurchaseOrder.Vendor)
                .Include(x => x.StockInProducts)
                .ToList();

            return View(stockIns);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult StockInForm()
        {
            StockInViewModel stockInViewModel = new StockInViewModel()
            {
                StockIn = new StockIn(),
                PurchaseOrders = db.PurchaseOrders.Include(x => x.Vendor).ToList()
            };

            return View(stockInViewModel);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult StockInFormSave(StockIn stockIn)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Bad Request. Fill all fields Correctly");
            }

            if (stockIn.PurchaseOrderId == 0)
                throw new HttpException(400, "Bad Request. PurchaseOrderId is required.");

            if (stockIn.StockInProducts == null || stockIn.StockInProducts.Any(x => x.ProductId == 0 || x.RequiredQty < 0 || x.UnitPrice < 1))
                throw new HttpException(400, "Bad Request. Select a valid product, Quantity, Cost and at least one product in request");

            stockIn.StockInProducts = stockIn.StockInProducts.Where(x => x.RequiredQty > 0).ToList();

            PurchaseOrder purchaseOrder = db.PurchaseOrders
                .Include(x => x.PurchaseOrderProducts)
                .SingleOrDefault(x => x.Id == stockIn.PurchaseOrderId);

            //Update Product Qty
            IEnumerable<Product> products = db.Products.ToList();
            foreach (StockInProduct stockInProduct in stockIn.StockInProducts)
            {
                //update Products Master Record
                Product p = products.SingleOrDefault(x => x.Id == stockInProduct.ProductId);
                if (p == null)
                    throw new HttpException(400, "Product Not Found");

                p.Cost = stockInProduct.UnitPrice;
                p.Quantity += stockInProduct.RequiredQty;

                //Update Purchase Order
                PurchaseOrderProduct POProduct = purchaseOrder
                    .PurchaseOrderProducts.SingleOrDefault(x => x.ProductId == stockInProduct.ProductId);

                if (POProduct == null)
                    throw new HttpException(400, "Product not found in purchase order.");

                if (POProduct.RemainingQty < stockInProduct.RequiredQty)
                    throw new HttpException(400, "RequiredQty must not be exceed RemainingQty.");

                POProduct.RemainingQty -= stockInProduct.RequiredQty;
            }

            db.StockIns.Add(stockIn);
            db.SaveChanges();

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult StockInDetail()
        {
            StockDetailViewModel stockDetailViewModel = new StockDetailViewModel()
            {
                Products = db.Products.ToList()
            };

            return View(stockDetailViewModel);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult StockInDetailReport(StockDetailViewModel stockDetailViewModel)
        {
            if (!ModelState.IsValid)
                return View("StockInDetail", stockDetailViewModel);

            Product product = db.Products.SingleOrDefault(x => x.Id == stockDetailViewModel.ProductId);

            if (product == null)
                return HttpNotFound();

            IQueryable<StockInProduct> stockInProducts = db.StockInProducts
                .Include(x => x.StockIn)
                .Where(x => x.ProductId == stockDetailViewModel.ProductId);
            
            if (stockDetailViewModel.DateFrom != null)
                stockInProducts = stockInProducts.Where(x => x.StockIn.InventoryDate >= stockDetailViewModel.DateFrom);
            if (stockDetailViewModel.DateTo != null)
                stockInProducts = stockInProducts.Where(x => x.StockIn.InventoryDate <= stockDetailViewModel.DateTo);

            product.StockInProducts = stockInProducts
                .OrderBy(x => x.StockIn.InventoryDate)
                .ToList();

            stockDetailViewModel.Product = product;

            return View(stockDetailViewModel);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult StockInDetailReportPdf(int productId, DateTime? dateFrom = null, DateTime? dateTo =null)
        {
            if (productId == 0)
                throw new HttpException(400, "Bad Request");

            Product product = db.Products.SingleOrDefault(x => x.Id == productId);

            if (product == null)
                return HttpNotFound();

            IQueryable<StockInProduct> stockInProducts = db.StockInProducts
                .Where(x => x.ProductId == productId);

            if (dateFrom != null)
                stockInProducts = stockInProducts.Where(x => x.StockIn.InventoryDate >= dateFrom);
            if (dateTo != null)
                stockInProducts = stockInProducts.Where(x => x.StockIn.InventoryDate <= dateTo);

            product.StockInProducts = stockInProducts
                .OrderBy(x => x.StockIn.InventoryDate)
                .ToList();

            //report
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/StockIn/StockInDetailReportPdf.rpt")));

            report.Database.Tables[0].SetDataSource(product.StockInProducts);
            report.Database.Tables[1].SetDataSource(new List<Product> { product });
            report.Database.Tables[2].SetDataSource(db.StockIns.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("StockInDetail_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
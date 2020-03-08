using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class StockOutController : Controller
    {
        private AppDbContext db;

        public StockOutController()
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
            IEnumerable<StockOut> stockOuts = db.StockOuts
                .Include(x => x.Complain.Client)
                .Include(x => x.StockOutProducts)
                .ToList();

            return View(stockOuts);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult NewStockOut()
        {
            return View(db.Complains.ToList());
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult StockOutForm(int Id)
        {
            Complain complain = db.Complains
                .Include(x => x.Client)
                .SingleOrDefault(x => x.Id == Id);

            if (complain == null)
                return HttpNotFound();

            return View(complain);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult StockOutFormSave(StockOut stockOut)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Bad Request. Fill all fields Correctly");
            }

            if (stockOut.ComplainId == 0)
                throw new HttpException(400, "Bad Request. ComplainId is required.");

            if (stockOut.StockOutProducts == null || stockOut.StockOutProducts.Any(x => x.ProductId == 0 || x.RequiredQty < 1 || x.UnitPrice < 1))
                throw new HttpException(400, "Bad Request. Select a valid product, Quantity, Cost and at least one product in request");

            //Update Product Qty
            IEnumerable<Product> products = db.Products.ToList();
            foreach (StockOutProduct stockProduct in stockOut.StockOutProducts)
            {
                Product p = products.SingleOrDefault(x => x.Id == stockProduct.ProductId);
                if (p == null)
                    throw new HttpException(400, "Product Not Found");

                if(stockProduct.RequiredQty > p.Quantity)
                    throw new HttpException(400, "Required quantity for product " + p.PrefixId + " is not available.");

                p.LastOutCost = stockProduct.UnitPrice;
                p.Quantity -= stockProduct.RequiredQty;
            }

            db.StockOuts.Add(stockOut);
            db.SaveChanges();

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult StockOutDetail()
        {
            StockDetailViewModel stockDetailViewModel = new StockDetailViewModel()
            {
                Products = db.Products.ToList()
            };

            return View(stockDetailViewModel);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult StockOutDetailReport(StockDetailViewModel stockDetailViewModel)
        {
            if (!ModelState.IsValid)
                return View("StockOutDetail", stockDetailViewModel);

            Product product = db.Products.SingleOrDefault(x => x.Id == stockDetailViewModel.ProductId);

            if (product == null)
                return HttpNotFound();

            IQueryable<StockOutProduct> stockInProducts = db.StockOutProducts
                .Include(x => x.StockOut.Complain.Client)
                .Where(x => x.ProductId == stockDetailViewModel.ProductId);

            if (stockDetailViewModel.DateFrom != null)
                stockInProducts = stockInProducts.Where(x => x.StockOut.StockIssueDateTime >= stockDetailViewModel.DateFrom);
            if (stockDetailViewModel.DateTo != null)
                stockInProducts = stockInProducts.Where(x => x.StockOut.StockIssueDateTime <= stockDetailViewModel.DateTo);

            product.StockOutProducts = stockInProducts
                .OrderBy(x => x.StockOut.StockIssueDateTime)
                .ToList();

            stockDetailViewModel.Product = product;

            return View(stockDetailViewModel);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult StockOutDetailReportPdf(int productId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            if (productId == 0)
                throw new HttpException(400, "Bad Request");

            Product product = db.Products.SingleOrDefault(x => x.Id == productId);

            if (product == null)
                return HttpNotFound();

            IQueryable<StockOutProduct> stockInProducts = db.StockOutProducts
                .Include(x => x.StockOut.Complain.Client)
                .Where(x => x.ProductId == productId);

            if (dateFrom != null)
                stockInProducts = stockInProducts.Where(x => x.StockOut.StockIssueDateTime >= dateFrom);
            if (dateTo != null)
                stockInProducts = stockInProducts.Where(x => x.StockOut.StockIssueDateTime <= dateTo);

            product.StockOutProducts = stockInProducts
                .OrderBy(x => x.StockOut.StockIssueDateTime)
                .ToList();

            //report
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/StockOut/StockOutDetailReportPdf.rpt")));

            report.Database.Tables[0].SetDataSource(product.StockOutProducts);
            report.Database.Tables[1].SetDataSource(new List<Product> { product });
            report.Database.Tables[2].SetDataSource(db.StockOuts.ToList());
            report.Database.Tables[3].SetDataSource(db.Complains.ToList());
            report.Database.Tables[4].SetDataSource(db.Clients.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("StockOutDetail_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
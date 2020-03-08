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
    public class SaleTexInvoiceController : Controller
    {
        private AppDbContext db;

        public SaleTexInvoiceController()
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
            IEnumerable<SalesTaxInvoice> salesTaxInvoices = db.SalesTaxInvoices.ToList();

            return View(salesTaxInvoices);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult SaleTexInvoiceForm()
        {
            SaleTexInvoiceViewModel saleTexInvoiceViewModel = new SaleTexInvoiceViewModel()
            {
                 PurchaseOrders = db.PurchaseOrders.ToList(),
                 Complains = db.Complains.ToList()
            };

            return View(saleTexInvoiceViewModel);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult SaleTaxInvoiceDetail(int id)
        {
            SalesTaxInvoice salesTaxInvoice = db.SalesTaxInvoices.Include(x => x.SaleTexInvoiceProducts).SingleOrDefault(x => x.Id == id);

            if (salesTaxInvoice == null)
                return HttpNotFound();

            SaleTexInvoiceViewModel saleTexInvoiceViewModel = new SaleTexInvoiceViewModel()
            {
                SalesTaxInvoice = salesTaxInvoice
            };

            return View(saleTexInvoiceViewModel);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult SaleTexInvoiceForm(SalesTaxInvoice salesTaxInvoice)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Bad Request. Fill all fields Correctly");
            }

            if (salesTaxInvoice.PurchaseOrderId == 0 || salesTaxInvoice.ComplainId == 0)
                throw new HttpException(400, "Bad Request. PurchaseRequestId and VendorId is required.");

            if (salesTaxInvoice.SaleTexInvoiceProducts == null || salesTaxInvoice.SaleTexInvoiceProducts.Any(x => x.ProductId == 0 || x.RequiredQty < 1 || x.UnitPrice < 1))
                throw new HttpException(400, "Bad Request. Select a valid product, Quantity, Cost and at least one product in request");

            db.SalesTaxInvoices.Add(salesTaxInvoice);
            db.SaveChanges();

            return Json(salesTaxInvoice.Id, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult SaleTaxInvoicePdf(int id)
        {
            SalesTaxInvoice salesTaxInvoice = db.SalesTaxInvoices
                .Include(x => x.SaleTexInvoiceProducts)
                .SingleOrDefault(x => x.Id == id);

            if (salesTaxInvoice == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/SaleTaxInvoice/SaleTaxInvoice.rpt")));

            report.Database.Tables[0].SetDataSource(new List<SalesTaxInvoice> { salesTaxInvoice });
            report.Database.Tables[1].SetDataSource(salesTaxInvoice.SaleTexInvoiceProducts);
            report.Database.Tables[2].SetDataSource(db.Products.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("SaleTaxInvoice_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
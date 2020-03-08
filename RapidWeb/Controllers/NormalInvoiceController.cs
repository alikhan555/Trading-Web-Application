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
    public class NormalInvoiceController : Controller
    {
        private AppDbContext db;

        public NormalInvoiceController()
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
            IEnumerable<NormalInvoice> normalInvoices = db.NormalInvoices.ToList();

            return View(normalInvoices);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult NormalInvoiceForm()
        {
            NormalInvoiceViewModel normalInvoiceViewModel = new NormalInvoiceViewModel()
            {
                PurchaseOrders = db.PurchaseOrders.ToList(),
                Complains = db.Complains.ToList()
            };

            return View(normalInvoiceViewModel);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Inventory)]
        public ActionResult NormalInvoiceForm(NormalInvoice normalInvoice)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Bad Request. Fill all fields Correctly");
            }

            if (normalInvoice.PurchaseOrderId == 0 || normalInvoice.ComplainId == 0)
                throw new HttpException(400, "Bad Request. PurchaseOrderId and ComplainId is required.");

            if (normalInvoice.NormalInvoiceProducts == null || normalInvoice.NormalInvoiceProducts.Any(x => x.ProductId == 0 || x.RequiredQty < 1 || x.UnitPrice < 1))
                throw new HttpException(400, "Bad Request. Select a valid product, Quantity, Cost and at least one product in request");

            db.NormalInvoices.Add(normalInvoice);
            db.SaveChanges();

            return Json(normalInvoice.Id, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult NormalInvoiceDetail(int id)
        {
            NormalInvoice normalInvoice = db.NormalInvoices.Include(x => x.NormalInvoiceProducts).SingleOrDefault(x => x.Id == id);

            if (normalInvoice == null)
                return HttpNotFound();

            NormalInvoiceViewModel normalInvoiceViewModel = new NormalInvoiceViewModel()
            {
                NormalInvoice = normalInvoice
            };

            return View(normalInvoiceViewModel);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Inventory, Role.RTPUser)]
        public ActionResult NormalInvoicePdf(int id)
        {
            NormalInvoice normalInvoice = db.NormalInvoices
                .Include(x => x.NormalInvoiceProducts)
                .SingleOrDefault(x => x.Id == id);

            if (normalInvoice == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/NormalInvoice/NormalInvoice.rpt")));

            report.Database.Tables[0].SetDataSource(new List<NormalInvoice> { normalInvoice });
            report.Database.Tables[1].SetDataSource(normalInvoice.NormalInvoiceProducts);
            report.Database.Tables[2].SetDataSource(db.Products.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("NormalInvoice_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
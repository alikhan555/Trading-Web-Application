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
    public class DailyExpenseController : Controller
    {
        private AppDbContext db;

        public DailyExpenseController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager, Role.RTPUser)]
        public ActionResult Index()
        {
            IEnumerable<DailyExpense> dailyExpenses = db.DailyExpenses
                .Include(x => x.Employee.Department)
                .ToList();

            return View(dailyExpenses);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager)]
        public ActionResult Create()
        {
            DailyExpenseViewModel dailyExpenseViewModel = new DailyExpenseViewModel()
            {
                Employees = db.Employees.Where(x => x.IsActive).ToList(),
            };

            return View(dailyExpenseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager)]
        public ActionResult Create(DailyExpense dailyExpense)
        {
            if (!ModelState.IsValid)
            {
                DailyExpenseViewModel dailyExpenseViewModel = new DailyExpenseViewModel()
                {
                    Employees = db.Employees.Where(x => x.IsActive).ToList(),
                    DailyExpense = dailyExpense
                };
                return View(dailyExpenseViewModel);
            }

            db.DailyExpenses.Add(dailyExpense);
            db.SaveChanges();

            return RedirectToAction("DailyExpenseDetail", new {id = dailyExpense.Id});
        }

        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager)]
        public ActionResult Edit(int id)
        {
            DailyExpense dailyExpense = db.DailyExpenses
                .Include(x => x.Employee.Department)
                .SingleOrDefault(x => x.Id == id);

            if (dailyExpense == null)
                return HttpNotFound();

            DailyExpenseViewModel dailyExpenseViewModel = new DailyExpenseViewModel()
            {
                Employees = db.Employees.Where(x => x.IsActive).ToList(),
                DailyExpense = dailyExpense
            };

            return View(dailyExpenseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager)]
        public ActionResult Edit(DailyExpense dailyExpense)
        {
            if (!ModelState.IsValid)
            {
                DailyExpenseViewModel dailyExpenseViewModel = new DailyExpenseViewModel()
                {
                    Employees = db.Employees.Where(x => x.IsActive).ToList(),
                    DailyExpense = dailyExpense
                };

                return View(dailyExpenseViewModel);
            }

            DailyExpense dailyExpenseInDb = db.DailyExpenses.SingleOrDefault(x => x.Id == dailyExpense.Id);

            if (dailyExpenseInDb == null)
                return HttpNotFound();

            dailyExpenseInDb.ExpenseVoucherDate = dailyExpense.ExpenseVoucherDate;
            dailyExpenseInDb.EmployeeId = dailyExpense.EmployeeId;
            dailyExpenseInDb.InAccountOf = dailyExpense.InAccountOf;
            dailyExpenseInDb.Amount = dailyExpense.Amount;

            db.SaveChanges();

            return RedirectToAction("DailyExpenseDetail", new { id = dailyExpense.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager, Role.RTPUser)]
        public ActionResult DailyExpenseDetail(int id)
        {
            DailyExpense dailyExpense = db.DailyExpenses
                .Include(x=>x.Employee.Department)
                .SingleOrDefault(x => x.Id == id);

            if (dailyExpense == null)
                return HttpNotFound();

            return View(dailyExpense);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager, Role.RTPUser)]
        public ActionResult DailyExpenseIndividualReport(int id)
        {
            DailyExpense dailyExpense = db.DailyExpenses
                .Include(x => x.Employee.Department)
                .SingleOrDefault(x => x.Id == id);

            if (dailyExpense == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/DailyExpense/DailyExpenseIndividualReport.rpt")));

            report.Database.Tables[0].SetDataSource(new List<DailyExpense> { dailyExpense });
            report.Database.Tables[1].SetDataSource(new List<Department> { dailyExpense.Employee.Department });
            report.Database.Tables[2].SetDataSource(new List<Employee> { dailyExpense.Employee});

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("DailyExpenseIndividual_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
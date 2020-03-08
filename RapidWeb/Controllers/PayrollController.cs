using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Asn1.X509;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class PayrollController : Controller
    {
        private AppDbContext db;

        public PayrollController()
        {
            db = new AppDbContext();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.RTPUser)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult NewPayrollFrom()
        {
            IEnumerable<Department> departments = db.Departments.Where(x => x.IsActive).ToList();

            return View(departments);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult SavePayroll(Payroll payroll)
        {
            if (!ModelState.IsValid)
                throw new HttpException(400, "Bad Request");

            if (db.Payrolls.Any(x => x.DepartmentId == payroll.DepartmentId && x.Month == payroll.Month && x.Year == payroll.Year))
                throw new HttpException(400, "Payroll already exist for this Department for this Month.");

            if (payroll.DepartmentId == 0)
                throw new HttpException(400, "Department is not selected.");

            if (payroll.EmployeeSalaries == null || payroll.EmployeeSalaries.Any(x => x.EmployeeId <= 0 || x.BasicSalary < 1))
                throw new HttpException(400, "Employee Salaries have bad value.");

            payroll.Date = DateTime.Now;

            db.Payrolls.Add(payroll);
            db.SaveChanges();

            return Json(new { });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.RTPUser)]
        public ActionResult SalaryDetailForm()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.RTPUser)]
        public ActionResult SalaryDetailPdf(int month = -1, int year = -1, int departmentIdFrom = -1, int departmentIdTo = -1)
        {
            if (month == -1 || year == -1)
            {
                return RedirectToAction("SalaryDetailForm");
            }

            IQueryable<EmployeeSalary> employeeSalariesQry = db.EmployeeSalaries.Where(x=> x.Payroll.Month == month && x.Payroll.Year == year);

            if (departmentIdFrom != -1)
                employeeSalariesQry = employeeSalariesQry.Where(x => x.Payroll.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                employeeSalariesQry = employeeSalariesQry.Where(x => x.Payroll.DepartmentId <= departmentIdTo);

            IEnumerable<EmployeeSalary> employeeSalaries = employeeSalariesQry.ToList();



            //IEnumerable<EmployeeSalary> employeeSalaries = db.EmployeeSalaries
            //    .Where(x =>
            //        x.Payroll.DepartmentId >= departmentIdFrom && x.Payroll.DepartmentId <= departmentIdTo &&
            //        x.Payroll.Month == month &&
            //        x.Payroll.Year == year).ToList();


            IQueryable<Payroll> payrollsQry = db.Payrolls.Where(x => x.Month == month && x.Year == year);

            if (departmentIdFrom != -1)
                payrollsQry = payrollsQry.Where(x => x.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                payrollsQry = payrollsQry.Where(x => x.DepartmentId <= departmentIdTo);

            IEnumerable<Payroll> payrolls = payrollsQry.ToList();

            //IEnumerable<Payroll> payrolls = db.Payrolls
            //    .Where(x => x.DepartmentId >= departmentIdFrom && x.DepartmentId <= departmentIdTo &&
            //                x.Month == month && x.Year == year).ToList();

            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Payroll/SalaryDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(employeeSalaries);
            report.Database.Tables[1].SetDataSource(payrolls);
            report.Database.Tables[2].SetDataSource(db.Departments.ToList());
            report.Database.Tables[3].SetDataSource(db.Employees.ToList());
            report.Database.Tables[4].SetDataSource(db.ServiceCities.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("SalaryDetailList_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult SalarySummeryPdf(int departmentIdFrom, int departmentIdTo, int month, int year)
        {
            IEnumerable<EmployeeSalary> employeeSalaries = db.EmployeeSalaries
                .Where(x =>
                    x.Payroll.DepartmentId >= departmentIdFrom && x.Payroll.DepartmentId <= departmentIdTo &&
                    x.Payroll.Month == month &&
                    x.Payroll.Year == year).ToList();

            IEnumerable<Payroll> payrolls = db.Payrolls
                .Where(x => x.DepartmentId >= departmentIdFrom && x.DepartmentId <= departmentIdTo &&
                            x.Month == month && x.Year == year).ToList();

            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Payroll/SalarySummaryReport.rpt")));

            report.Database.Tables[0].SetDataSource(employeeSalaries);
            report.Database.Tables[1].SetDataSource(payrolls);
            report.Database.Tables[2].SetDataSource(db.Departments.ToList());


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("SalarySummaryReport_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.RTPUser)]
        public ActionResult PaySlipForm()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.RTPUser)]
        public ActionResult GeneratePaySlips(int month = -1, int year = -1, int departmentIdFrom = -1, int departmentIdTo = -1, int employeeIdFrom = -1, int employeeIdTo = -1)
        {
            if (month == -1 || year == -1)
            {
                return RedirectToAction("PaySlipForm");
            }

            IQueryable<EmployeeSalary> emlEmployeeSalariesQry =
                db.EmployeeSalaries.Where(x => x.Payroll.Month == month && x.Payroll.Year == year);

            if (departmentIdFrom != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.Payroll.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.Payroll.DepartmentId <= departmentIdTo);

            if (employeeIdFrom != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.EmployeeId >= employeeIdFrom);
            if (employeeIdTo != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.EmployeeId <= employeeIdTo);

            IEnumerable<EmployeeSalary> employeeSalaries = emlEmployeeSalariesQry.ToList();

            IQueryable<Payroll> payrollQry =
                db.Payrolls.Where(x => x.Month == month && x.Year == year);

            if (departmentIdFrom != -1)
                payrollQry = payrollQry.Where(x => x.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                payrollQry = payrollQry.Where(x => x.DepartmentId <= departmentIdTo);
            
            IEnumerable<Payroll> payrolls = payrollQry.ToList();
            
            //pdf
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Payroll/PaySlips.rpt")));

            report.Database.Tables[0].SetDataSource(employeeSalaries);
            report.Database.Tables[1].SetDataSource(payrolls);
            report.Database.Tables[2].SetDataSource(db.Departments.ToList());
            report.Database.Tables[3].SetDataSource(db.Employees.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("PaySlips_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
        
        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult GeneratePayrollForm()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult GetCompleteSalaries(int month, int year, int departmentIdFrom = -1, int departmentIdTo = -1, int employeeIdFrom = -1, int employeeIdTo = -1)
        {
            IQueryable<EmployeeSalary> emlEmployeeSalariesQry =
                db.EmployeeSalaries.Where(x => x.Payroll.Month == month && x.Payroll.Year == year);

            if (departmentIdFrom != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.Payroll.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.Payroll.DepartmentId <= departmentIdTo);

            if (employeeIdFrom != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.EmployeeId >= employeeIdFrom);
            if (employeeIdTo != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.EmployeeId <= employeeIdTo);

            IEnumerable<EmployeeSalary> employeeSalaries = emlEmployeeSalariesQry
                .Include(x => x.Employee.City)
                .Include(x => x.Payroll.Department)
                .ToList();

            return Json(employeeSalaries, JsonRequestBehavior.AllowGet);
        }
        
        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult GeneratePayrollView(PayrollGenerate payrollGenerate)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("GeneratePayrollForm");
            }

            payrollGenerate.EmployeeSalaries = db.EmployeeSalaries
                .Include(x => x.Employee.City)
                .Include(x => x.Payroll.Department)
                .Where(x =>
                    x.Payroll.DepartmentId >= payrollGenerate.DepartmentIdFrom &&
                    x.Payroll.DepartmentId <= payrollGenerate.DepartmentIdTo &&
                    x.Payroll.Month == payrollGenerate.Month.Month &&
                    x.Payroll.Year == payrollGenerate.Month.Year);

            return View(payrollGenerate);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.RTPUser)]
        public ActionResult PaySlipsView(int month = -1, int year = -1, int departmentIdFrom = -1, int departmentIdTo = -1, int employeeIdFrom = -1, int employeeIdTo = -1)
        {
            if (month == -1 || year == -1)
            {
                return RedirectToAction("PaySlipForm");
            }

            IQueryable<EmployeeSalary> emlEmployeeSalariesQry = db.EmployeeSalaries
                .Include(x => x.Payroll.Department)
                .Include(x => x.Employee)
                .Where(x => x.Payroll.Month == month && x.Payroll.Year == year);

            if (departmentIdFrom != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.Payroll.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.Payroll.DepartmentId <= departmentIdTo);

            if (employeeIdFrom != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.EmployeeId >= employeeIdFrom);
            if (employeeIdTo != -1)
                emlEmployeeSalariesQry = emlEmployeeSalariesQry.Where(x => x.EmployeeId <= employeeIdTo);

            IEnumerable<EmployeeSalary> employeeSalaries = emlEmployeeSalariesQry.ToList();
            
            return View(employeeSalaries);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.RTPUser)]
        public ActionResult SalaryDetails(int month = -1, int year = -1, int departmentIdFrom = -1, int departmentIdTo = -1)
        {
            if (month == -1 || year == -1)
            {
                return RedirectToAction("SalaryDetailForm");
            }

            IQueryable<EmployeeSalary> employeeSalariesQry = db.EmployeeSalaries
                .Include(x => x.Payroll.Department)
                .Include(x => x.Employee.City)
                .Where(x => x.Payroll.Month == month && x.Payroll.Year == year);

            if (departmentIdFrom != -1)
                employeeSalariesQry = employeeSalariesQry.Where(x => x.Payroll.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                employeeSalariesQry = employeeSalariesQry.Where(x => x.Payroll.DepartmentId <= departmentIdTo);

            IEnumerable<EmployeeSalary> employeeSalaries = employeeSalariesQry.ToList();

            return View(employeeSalaries);
        }
    }
}
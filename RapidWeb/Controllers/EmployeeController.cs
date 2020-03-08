using System;
using RapidWeb.Models;
using RapidWeb.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;

namespace RapidWeb.Controllers
{
    public class EmployeeController : Controller
    {
        private AppDbContext db;

        public EmployeeController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult Index()
        {
            IEnumerable<Employee> employees = db.Employees
                .Include(x => x.Department)
                .Include(x=>x.City)
                .ToList();

            return View(employees);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult GetEmployeesByDepartment(int id)             //Id = DepartmentID
        {
            IEnumerable<Employee> employees = db.Employees
                .Where(x => x.IsActive)
                .Include(x => x.City)
                .Where(x => x.DepartmentId == id)
                .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult Create()
        {
            EmployeeViewModel employeeViewModel = new EmployeeViewModel()
            {
                Departments = db.Departments.Where(x => x.IsActive).ToList(),
                ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList()
            };

            return View(employeeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult Create(EmployeeViewModel employeeViewModel)
        {
            if (!ModelState.IsValid)
            {
                employeeViewModel.Departments = db.Departments.Where(x => x.IsActive).ToList();
                employeeViewModel.ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList();

                return View(employeeViewModel);
            }

            Employee employee = employeeViewModel.Employee;
            employee.IsActive = true;

            db.Employees.Add(employee);
            db.SaveChanges();

            return RedirectToAction("EmployeeDetail","Employee", new {id = employee.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult Edit(int id)
        {
            Employee employee = db.Employees.SingleOrDefault(x => x.Id == id);

            if (employee == null)
                return HttpNotFound();

            EmployeeViewModel employeeViewModel = new EmployeeViewModel()
            {
                Employee = employee,
                Departments = db.Departments.Where(x => x.IsActive).ToList(),
                ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList()
            };

            return View(employeeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult Edit(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                EmployeeViewModel employeeViewModel = new EmployeeViewModel()
                {
                    Employee = employee,
                    Departments = db.Departments.Where(x => x.IsActive).ToList(),
                    ServiceCities = db.ServiceCities.Where(x => x.IsActive).ToList()
                };
                return View(employeeViewModel);
            }

            Employee employeeInDb = db.Employees.SingleOrDefault(x => x.Id == employee.Id);

            if (employeeInDb == null)
                return HttpNotFound();

            employeeInDb.Name = employee.Name;
            employeeInDb.Guardian = employee.Guardian;
            employeeInDb.Address = employee.Address;
            employeeInDb.CNIC = employee.CNIC;
            employeeInDb.ContactMobile = employee.ContactMobile;
            employeeInDb.ContactHome = employee.ContactHome;
            employeeInDb.Dob = employee.Dob;
            employeeInDb.Gender = employee.Gender;
            employeeInDb.Qualification = employee.Qualification;
            employeeInDb.DepartmentId = employee.DepartmentId;
            employeeInDb.Designation = employee.Designation;
            employeeInDb.Experience = employee.Experience;
            employeeInDb.DateOfJoining = employee.DateOfJoining;
            employeeInDb.Salary = employee.Salary;
            employeeInDb.CityId = employee.CityId;
            employeeInDb.HouseRent = employee.HouseRent;
            employeeInDb.TransportAllowance = employee.TransportAllowance;
            employeeInDb.UtilityAllowance = employee.UtilityAllowance;
            employeeInDb.BonusAllowance = employee.BonusAllowance;
            employeeInDb.OtherBenefits = employee.OtherBenefits;
            employeeInDb.BankAccountDetail = employee.BankAccountDetail;
            employeeInDb.Note = employee.Note;

            db.SaveChanges();

            return RedirectToAction("EmployeeDetail", "Employee", new { id = employee.Id });
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult ChangeActiveStatus(int id)
        {
            Employee employee = db.Employees.SingleOrDefault(x => x.Id == id);

            if (employee == null)
                return HttpNotFound();

            employee.IsActive = !employee.IsActive;

            db.SaveChanges();

            return RedirectToAction("index", "Employee");
        }
        
        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult EmployeeDetail(int id)
        {
            Employee employee = db.Employees
                .Include(x => x.Department)
                .Include(x => x.City)
                .SingleOrDefault(x => x.Id == id);

            if (employee == null)
                return HttpNotFound();

            return View(employee);
        }
        
        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult EmployeesReport(int empIdFrom = -1, int empIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1, int departmentIdFrom = -1, int departmentIdTo = -1)
        {
            IQueryable<Employee> employeesQry = db.Employees;

            if (empIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.Id >= empIdFrom);
            if (empIdTo != -1)
                employeesQry = employeesQry.Where(x => x.Id <= empIdTo);

            if (cityIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.CityId >= cityIdFrom);
            if (cityIdTo != -1)
                employeesQry = employeesQry.Where(x => x.CityId <= cityIdTo);

            if (departmentIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                employeesQry = employeesQry.Where(x => x.DepartmentId <= departmentIdTo);

            List<Employee> employees = employeesQry.Include(x => x.Department).ToList();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Employee/EmployeesDetailReport.rpt")));

            report.Database.Tables[0].SetDataSource(employeesQry.Include(x => x.Department).ToList());
            report.Database.Tables[1].SetDataSource(db.Departments.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("EmployeesList_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult GetEmployees(int empIdFrom = -1, int empIdTo = -1, int cityIdFrom = -1, int cityIdTo = -1, int departmentIdFrom = -1, int departmentIdTo = -1)
        {
            IQueryable<Employee> employeesQry = db.Employees;

            if (empIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.Id >= empIdFrom);
            if (empIdTo != -1)
                    employeesQry = employeesQry.Where(x => x.Id <= empIdTo);

            if (cityIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.CityId >= cityIdFrom);
            if (cityIdTo != -1)
                employeesQry = employeesQry.Where(x => x.CityId <= cityIdTo);

            if (departmentIdFrom != -1)
                employeesQry = employeesQry.Where(x => x.DepartmentId >= departmentIdFrom);
            if (departmentIdTo != -1)
                employeesQry = employeesQry.Where(x => x.DepartmentId <= departmentIdTo);

            List<Employee> employees = employeesQry.Include(x=> x.Department).ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR, Role.AccountsManager)]
        public ActionResult GetEmployee(int id)
        {
            Employee employee = db.Employees.Include(x => x.Department).SingleOrDefault(x => x.Id == id);
            if (employee == null)
                return HttpNotFound();

            return Json(employee, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Role.SuperAdmin, Role.HR)]
        public ActionResult EmployeeIndividualReport(int id)
        {
            Employee employee = db.Employees.SingleOrDefault(x => x.Id == id);

            if (employee == null)
                return HttpNotFound();

            //report code
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Employee/EmployeeIndividualReport.rpt")));

            report.Database.Tables[0].SetDataSource(new List<Employee> { employee });
            report.Database.Tables[1].SetDataSource(db.Departments.ToList());
            report.Database.Tables[2].SetDataSource(db.ServiceCities.ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            string savedFileName = string.Format("EmployeeIndividual_{0}.pdf", DateTime.Now);
            return File(stream, "application/pdf", savedFileName);
        }
    }
}
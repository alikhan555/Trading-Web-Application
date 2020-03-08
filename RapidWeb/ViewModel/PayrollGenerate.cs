using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class PayrollGenerate
    {
        public DateTime Month { get; set; }

        public int DepartmentIdFrom { get; set; }

        public int DepartmentIdTo { get; set; }

        public IEnumerable<EmployeeSalary> EmployeeSalaries { get; set; }
    }
}
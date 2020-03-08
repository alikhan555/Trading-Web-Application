using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class EmployeeViewModel
    {
        public Employee Employee { get; set; }

        public IEnumerable<Department> Departments { get; set; }

        public IEnumerable<ServiceCity> ServiceCities { get; set; }
    }
}
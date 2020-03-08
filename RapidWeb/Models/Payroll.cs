using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Payroll
    {
        [Display(Name = "Payroll ID")]
        public int Id { get; set; }

        [ForeignKey("Department")]
        [Display(Name = "Department ID")]
        public int DepartmentId { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Range(2000, 2099)]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Range(1, 12)]
        [Display(Name = "Month")]
        public int Month { get; set; }



        public Department Department { get; set; }
        public ICollection<EmployeeSalary> EmployeeSalaries { get; set; }
    }
}
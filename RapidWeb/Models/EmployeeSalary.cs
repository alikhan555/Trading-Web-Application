using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class EmployeeSalary
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Payroll")]
        [Display(Name = "Payroll ID")]
        public int PayrollId { get; set; }

        [ForeignKey("Employee")]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }

        [Display(Name = "Basic Salary")]
        public double BasicSalary { get; set; }

        [Display(Name = "Housing Allowance")]
        public double HousingAllowance { get; set; }

        [Display(Name = "Utility Allowance")]
        public double UtilityAllowance { get; set; }

        [Display(Name = "Transport Allowance")]
        public double TransportAllowance { get; set; }

        [Display(Name = "Bonus Allowance")]
        public double BonusAllowance { get; set; }

        [Display(Name = "Other Allowance")]
        public double OtherAllowance { get; set; }

        [Display(Name = "Tax Deduction")]
        public double TaxDeduction { get; set; }

        [Display(Name = "Other Deduction")]
        public double OtherDeduction { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        
        [NotMapped]
        [Display(Name = "Total Allowance")]
        public double TotalAllowance
        {
            get { return HousingAllowance + UtilityAllowance + TransportAllowance + BonusAllowance + OtherAllowance; }
        }

        [NotMapped]
        [Display(Name = "Total Deduction")]
        public double TotalDeduction
        {
            get { return TaxDeduction + OtherDeduction; }
        }

        [NotMapped]
        [Display(Name = "Total Salary")]
        public double TotalSalary
        {
            get { return BasicSalary + TotalAllowance - TotalDeduction; }
        }



        public Payroll Payroll { get; set; }
        public Employee Employee { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class DailyExpense
    {
        [Display(Name = "Expense Voucher ID")]
        public int Id { get; set; }

        [Display(Name = "Expense Voucher Date")]
        public DateTime ExpenseVoucherDate { get; set; }

        [ForeignKey("Employee")]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "In Account Of")]
        public string InAccountOf { get; set; }

        [Display(Name = "Amount")]
        public double Amount { get; set; }



        public Employee Employee { get; set; }



        public const string Prefix = "EV";

        [NotMapped]
        [Display(Name = "Expense Voucher ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
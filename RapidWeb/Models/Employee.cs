using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Server;

namespace RapidWeb.Models
{
    public class Employee
    {
        [Key]
        [Display(Name = "Employee ID")]
        public int Id { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Employee Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Guardian")]
        public string Guardian { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [StringLength(15)]
        [Display(Name = "Employee CNIC")]
        public string CNIC { get; set; }

        [Required]
        [StringLength(17)]
        [Display(Name = "Employee Mobile Contact")]
        public string ContactMobile { get; set; }

        [Required]
        [StringLength(17)]
        [Display(Name = "Employee Home Contact")]
        public string ContactHome { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Employee Date Of Birth")]
        public DateTime Dob { get; set; }

        [Required]
        [StringLength(6)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [StringLength(maximumLength:50, MinimumLength = 2)]
        [Display(Name = "Qualification")]
        public string Qualification { get; set; }

        [Required]
        [ForeignKey("Department")]
        [Display(Name = "Department ID")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Experience")]
        [StringLength(40)]
        public string Experience { get; set; }

        [Display(Name = "Date Of Join")]
        public DateTime DateOfJoining { get; set; }

        [Display(Name = "Salary")]
        public double Salary { get; set; }

        [ForeignKey("City")]
        [Display(Name = "City ID")]
        public int CityId { get; set; }

        [Display(Name = "House Rent")]
        public double HouseRent { get; set; }

        [Display(Name = "Transport Allowance")]
        public double TransportAllowance { get; set; }

        [Display(Name = "Utility Allowance")]
        public double UtilityAllowance { get; set; }

        [Display(Name = "Bonus Allowance")]
        public double BonusAllowance { get; set; }

        [Display(Name = "Other Benefits")]
        public double OtherBenefits { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [NotMapped]
        [Display(Name = "Gross Salary")]
        public double GrossSalary
        {
            get { return Salary + HouseRent + TransportAllowance + UtilityAllowance +BonusAllowance + OtherBenefits; }      
        }

        [Required]
        [Display(Name = "Bank Account Details")]
        public string BankAccountDetail { get; set; }

        [Required]
        [Display(Name = "Note")]
        public string Note { get; set; }



        public ServiceCity City { get; set; }
        public Department Department { get; set; }



        public const string Prefix = "EMP";

        [NotMapped]
        [Display(Name = "Employee ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
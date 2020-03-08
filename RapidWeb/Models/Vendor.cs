using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Vendor
    {
        [Display(Name = "Vendor ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vendor Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }     // credit or debit

        [StringLength(12, MinimumLength = 7)]
        [Display(Name = "NTN No.")]
        public string NTNNo { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 11)]
        [Display(Name = "Contact No.")]
        public string Contact { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        public const string Prefix = "SUP";

        [NotMapped]
        [Display(Name = "Vendor ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Service
    {
        [Display(Name = "Service ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Package Type")]
        public string PackageType { get; set; }

        [Display(Name = "Maintenance")]
        public bool Maintenance { get; set; }

        [Display(Name = "Installation")]
        public bool Installation { get; set; }

        [Display(Name = "Development")]
        public bool Development { get; set; }

        [Display(Name = "Troubleshooting")]
        public bool Troubleshooting { get; set; }

        [Display(Name = "Networking")]
        public bool Networking { get; set; }

        [Display(Name = "Other")]
        public bool Other { get; set; }

        [Required]
        [Display(Name = "Package Details")]
        public string PackageDetails { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public const string Prefix = "SRV";

        [NotMapped]
        [Display(Name = "Service ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
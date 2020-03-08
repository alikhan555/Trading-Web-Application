using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Package
    {
        [Display(Name = "Package ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Package Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Package Detail")]
        public string Detail { get; set; }

        [Required]
        [Display(Name = "Job")]
        public string Job { get; set; }

        [Required]
        [Display(Name = "Customized Detail")]
        public string CustomizeDetail { get; set; }

        [Required]
        [Display(Name = "Reference Note")]
        public string ReferenceNote { get; set; }

        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public const string Prefix = "PKG";

        [NotMapped]
        [Display(Name = "Package ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
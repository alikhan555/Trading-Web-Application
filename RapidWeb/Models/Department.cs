using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Department
    {
        [Display(Name = "Department ID")]
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Department Name")]
        public string Name { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public const string Prefix = "DPT";

        [NotMapped]
        [Display(Name = "Department ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
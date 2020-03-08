using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class ServiceCity
    {
        [Display(Name = "City Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "City Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "District Name")]
        public string District { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public const string Prefix = "CITY";

        [NotMapped]
        [Display(Name = "City Id")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
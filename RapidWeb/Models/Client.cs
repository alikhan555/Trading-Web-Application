using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Client
    {
        [Key]
        [Display(Name = "Client ID")]
        public int Id { get; set; }

        [Display(Name = "Ref Code")]
        public int RefCode { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Client Name")]
        public string Name { get; set; }

        [Required]
        [ForeignKey("City")]
        [Display(Name = "City")]
        public int CityId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Area")]
        public string Area { get; set; }

        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPersonName { get; set; }

        [Required]
        [StringLength(17)]
        [Display(Name = "Mobile No.")]
        public string ContactNo { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 12)]
        [Display(Name = "Landline No.")]
        public string ContactLandLine { get; set; }

        [Required]
        [ForeignKey("Package")]
        [Display(Name = "Package ID")]
        public int PackageId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "On Days")]
        public string OnDays { get; set; }

        [Display(Name = "Timing")]
        public string Timing { get; set; }

        [Display(Name = "Credit Days")]
        public int CreditDays { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public ServiceCity City { get; set; }
        public Package Package { get; set; }



        public const string Prefix = "CL";

        [NotMapped]
        [Display(Name = "Client ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
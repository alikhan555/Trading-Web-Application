using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Complain
    {
        [Display(Name = "Complain ID")]
        public int Id { get; set; }

        [ForeignKey("Client")]
        [Display(Name = "Client ID")]
        public int ClientId { get; set; }

        //public int InvCode { get; set; }

        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDateTime { get; set; }

        [Display(Name = "Completion Date")]
        public DateTime CompletionDateTime { get; set; }

        [Required]
        [Display(Name = "Complain Reference")]
        public string ComplainReference { get; set; }

        [Required]
        [Display(Name = "Complain Type")]
        public string ComplainType { get; set; }                // dropdown with list

        [Display(Name = "Progress Report")]
        public string ProgressReport { get; set; }

        [Required]
        [ForeignKey("Employee")]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }              // EmployeeId

        [Required]
        [ForeignKey("Service")]
        [Display(Name = "Service ID")]
        public int ServiceId { get; set; }                      //services DropDown

        [Required]
        [Display(Name = "Complain Nature")]
        public string ComplainNature { get; set; }

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }




        public Client Client { get; set; }
        public Employee Employee { get; set; }
        public Service Service { get; set; }
        public ICollection<ComplainProductsDetail> AssignmentProducts { get; set; }




        public const string Prefix = "TKT";

        [NotMapped]
        [Display(Name = "Complain ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class NormalInvoice
    {
        [Key]
        [Display(Name = "Inv#")]
        public int Id { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Complain Id")]
        public int ComplainId { get; set; }

        [Display(Name = "Product Order Id")]
        public int PurchaseOrderId { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Customer Address")]
        public string CustomerAddress { get; set; }
        
        [Required]
        [Display(Name = "Areesh & Co. Address")]
        public string CoAddress { get; set; }

        [Required]
        [Display(Name = "Nature Of Work")]
        public string WorkNature { get; set; }

        [Required]
        [Display(Name = "Total In Words")]
        public string InWords { get; set; }
        
        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }



        public ICollection<NormalInvoiceProduct> NormalInvoiceProducts { get; set; }



        public const string Prefix = "R";

        [NotMapped]
        [Display(Name = "Inv#")]
        public string PrefixId
        {
            get { return Prefix + Id + "-" + Date.ToString("yy"); }
        }
    }
}
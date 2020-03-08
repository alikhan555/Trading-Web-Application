using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class SalesTaxInvoice
    {
        [Key]
        [Display(Name = "Inv#")]
        public int Id { get; set; }                 //InvNo

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

        [StringLength(15, MinimumLength = 10)]
        [Display(Name = "PSTR No.")]
        public string PSTRNo { get; set; }

        [StringLength(12, MinimumLength = 7)]
        [Display(Name = "NTN No.")]
        public string NTNNo { get; set; }

        [Required]
        [Display(Name = "Areesh &  Co. Address")]
        public string CoAddress { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 10)]
        [Display(Name = "Co. PSTR No.")]
        public string CoPSTRNo { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 7)]
        [Display(Name = "Co. NTN No.")]
        public string CoNTNNo { get; set; }

        [Required]
        [Display(Name = "Nature Of Work")]
        public string WorkNature { get; set; }

        [Required]
        [Display(Name = "Total In Words")]
        public string InWords { get; set; }

        [Display(Name = "Total Excluding Sale Tax")]
        public double TotalExcludingSaleTex { get; set; }

        [Display(Name = "Sale Tax Amount Payable")]
        public double SaleTexAmountPayable { get; set; }

        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }



        //public Complain Complain { get; set; }
        //public PurchaseOrder PurchaseOrder { get; set; }
        public ICollection<SaleTexInvoiceProduct> SaleTexInvoiceProducts { get; set; }

        public const string Prefix = "ST";

        [NotMapped]
        [Display(Name = "Inv#")]
        public string PrefixId
        {
            get { return Prefix + Id + "-" + Date.ToString("yy"); }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class PurchaseOrder
    {
        [Display(Name = "Purchase Order ID")]
        public int Id { get; set; }

        [ForeignKey("PurchaseRequest")]
        [Display(Name = "Purchase Request ID")]
        public int PurchaseRequestId { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDateTime { get; set; }

        [ForeignKey("Vendor")]
        [Display(Name = "Vendor ID")]
        public int VendorId { get; set; }



        public ICollection<PurchaseOrderProduct> PurchaseOrderProducts { get; set; }
        public Vendor Vendor { get; set; }
        public PurchaseRequest PurchaseRequest { get; set; }

        [NotMapped]
        [Display(Name = "Total Amount")]
        public double TotalAmount
        {
            get
            {
                if (PurchaseOrderProducts == null || PurchaseOrderProducts.Count == 0)
                    return 0;

                return PurchaseOrderProducts.Select(x => new { x = x.UnitPrice * x.RequiredQty }).Sum(x => x.x);
            }
        }



        public const string Prefix = "PO";

        [NotMapped]
        [Display(Name = "Purchase Order ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
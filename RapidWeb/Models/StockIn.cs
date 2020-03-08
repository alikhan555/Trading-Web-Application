using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class StockIn
    {
        [Display(Name = "Stock-In ID")]
        public int Id { get; set; }

        [ForeignKey("PurchaseOrder")]
        [Display(Name = "Purchase Order ID")]
        public int PurchaseOrderId { get; set; }

        [Display(Name = "Inventory Date")]
        public DateTime InventoryDate { get; set; }
        
        public ICollection<StockInProduct> StockInProducts { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }



        public const string Prefix = "STI";

        [NotMapped]
        [Display(Name = "Stock-In ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
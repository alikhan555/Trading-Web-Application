using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class PurchaseRequestProduct
    {
        public int Id { get; set; }

        [ForeignKey("PurchaseRequest")]
        public int PurchaseRequestId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Range(1, 9999999)]
        public int RequiredQty { get; set; }

        [Range(1, 999999)]
        public double UnitPrice { get; set; }

        [Range(0, 9999999)]
        public int RemainingQty { get; set; }



        public PurchaseRequest PurchaseRequest { get; set; }
        public Product Product { get; set; }
    }
}
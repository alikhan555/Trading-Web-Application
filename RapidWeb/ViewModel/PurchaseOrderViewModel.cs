using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class PurchaseOrderViewModel
    {
        public PurchaseOrder PurchaseOrder { get; set; }

        public ICollection<Vendor> Vendors { get; set; }

        public IEnumerable<PurchaseRequest> PurchaseRequests { get; set; }
    }
}
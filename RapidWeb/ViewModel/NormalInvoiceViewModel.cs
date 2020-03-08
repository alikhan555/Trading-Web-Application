using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class NormalInvoiceViewModel
    {
        public IEnumerable<Complain> Complains { get; set; }

        public IEnumerable<PurchaseOrder> PurchaseOrders { get; set; }

        public NormalInvoice NormalInvoice { get; set; }
    }
}
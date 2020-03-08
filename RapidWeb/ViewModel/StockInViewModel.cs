using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class StockInViewModel
    {
        public StockIn StockIn { get; set; }

        public IEnumerable<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
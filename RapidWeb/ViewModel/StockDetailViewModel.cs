using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class StockDetailViewModel
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        [Display(Name = "Product Id")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
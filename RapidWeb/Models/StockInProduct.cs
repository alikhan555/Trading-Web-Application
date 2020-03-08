using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class StockInProduct
    {
        public int Id { get; set; }

        [ForeignKey("StockIn")]
        public int StockInId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Range(0, 9999999)]
        public int RequiredQty { get; set; }

        [Range(1, 999999)]
        public double UnitPrice { get; set; }


        public StockIn StockIn { get; set; }
        public Product Product { get; set; }
    }
}
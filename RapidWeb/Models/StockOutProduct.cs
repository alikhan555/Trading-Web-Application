﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class StockOutProduct
    {
        public int Id { get; set; }

        [ForeignKey("StockOut")]
        public int StockOutId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Range(1, 9999999)]
        public int RequiredQty { get; set; }

        [Range(1, 999999)]
        public double UnitPrice { get; set; }


        public StockOut StockOut { get; set; }
        public Product Product { get; set; }
    }
}
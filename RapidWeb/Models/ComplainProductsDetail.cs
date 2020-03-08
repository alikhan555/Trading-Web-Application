using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class ComplainProductsDetail
    {
        public int Id { get; set; }

        [ForeignKey("Complain")]
        public int ComplainId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Qty { get; set; }

        public double Cost { get; set; }



        public Complain Complain { get; set; }
        public Product Product { get; set; }
    }
}
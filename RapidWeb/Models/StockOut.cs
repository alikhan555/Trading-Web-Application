using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class StockOut
    {
        public int Id { get; set; }

        [ForeignKey("Complain")]
        public int ComplainId { get; set; }

        public DateTime StockIssueDateTime { get; set; }

        public ICollection<StockOutProduct> StockOutProducts { get; set; }
        public Complain Complain { get; set; }


        public const string Prefix = "STO";

        [NotMapped]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
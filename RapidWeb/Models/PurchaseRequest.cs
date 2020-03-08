using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class PurchaseRequest
    {
        [Display(Name = "Purchase Request ID")]
        public int Id { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreationDateTime { get; set; }

        [Display(Name = "Request For")]
        public string RequestFor { get; set; }

        public ICollection<PurchaseRequestProduct> PurchaseRequestProducts { get; set; }



        public const string Prefix = "PR";

        [NotMapped]
        [Display(Name = "Purchase Request ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
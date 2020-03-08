using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Product
    {
        [Display(Name = "Product ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }

        [Display(Name = "Purchasing Date")]
        public DateTime CreationDate { get; set; }

        [Range(1, 9999999)]
        [Display(Name = "Cost")]
        public double Cost { get; set; }

        [Range(0, 9999999)]
        [Display(Name = "Last Sold Cost")]
        public double LastOutCost { get; set; }

        [Range(0, 999999)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Range(0, 999999)]
        [Display(Name = "Sample Quantity")]
        public int SampleQuantity { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public IEnumerable<StockInProduct> StockInProducts { get; set; }
        public IEnumerable<StockOutProduct> StockOutProducts { get; set; }



        public const string Prefix = "PRD";

        [NotMapped]
        [Display(Name = "Product ID")]
        public string PrefixId
        {
            get { return Prefix + Id; }
        }
    }
}
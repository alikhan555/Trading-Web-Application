using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class ComplainViewModel
    {
        [Required]
        public Complain Complain { get; set; }

        public Client Client { get; set; }

        public IEnumerable<Employee> Employee { get; set; }

        public IEnumerable<Service> Service { get; set; }
    }
}
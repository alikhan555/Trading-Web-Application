using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class ClientViewModel
    {
        public Client Client { get; set; }

        public IEnumerable<ServiceCity> ServiceCities { get; set; }

        public IEnumerable<Package> Packages { get; set; }
    }
}
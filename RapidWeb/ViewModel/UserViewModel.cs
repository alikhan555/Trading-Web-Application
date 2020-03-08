using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class UserViewModel
    {
        public User User { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}
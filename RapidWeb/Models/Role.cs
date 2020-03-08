using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }



        public const int SuperAdmin = 1;
        public const int Admin = 2;
        public const int DataOperator = 3;
        public const int HR = 4;
        public const int Inventory = 5;
        public const int RTPUser = 6;
        public const int AccountsManager = 7;
        public const int Activity = 8;
    }
}
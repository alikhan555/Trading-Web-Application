using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 5)]
        [Index(IsUnique = true)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 5)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}
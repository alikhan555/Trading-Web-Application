using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RapidWeb.ViewModel
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Current password")]
        [StringLength(maximumLength: 256, MinimumLength = 5)]
        public string CurrentPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        [StringLength(maximumLength: 256, MinimumLength = 5)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Re-type new password")]
        [StringLength(maximumLength: 256, MinimumLength = 5)]
        public string ConfirmNewPassword { get; set; }
    }
}
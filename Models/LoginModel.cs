using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eCommerceWebsite_1912C1.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "you must enter your Email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "you must enter your Password!")]
        public string Password { get; set; }
    }
}
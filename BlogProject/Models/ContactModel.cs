using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace MyPortfolio.Models
{
    public class ContactModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Email:")]
        public String email { get; set; }

        [Required]
        [Display(Name = "Name:")]
        public String name { get; set; }

        [Required]
        [Display(Name = "Message:")]
        public String message { get; set; }
    }
}
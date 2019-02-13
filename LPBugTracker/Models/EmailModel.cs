using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LPBugTracker.Models
{
    public class EmailModel
    {
        
        [Required, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }
        
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string ToName { get; set; }

        [Required, EmailAddress]
        public string ToEmail { get; set; }
    }
}
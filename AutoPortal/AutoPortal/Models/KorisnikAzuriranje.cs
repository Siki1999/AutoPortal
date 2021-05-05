using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoPortal.Models
{
    public class KorisnikAzuriranje
    {
        [Display(Name = "Korisničko ime")]
        [Required]
        public string KorisnickoIme { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Prezime")]
        [Required]
        public string Prezime { get; set; }

        [Display(Name = "Ime")]
        [Required]
        public string Ime { get; set; }

        [Display(Name = "Ovlast")]
        [Required]
        public string Ovlast { get; set; }
    }
}
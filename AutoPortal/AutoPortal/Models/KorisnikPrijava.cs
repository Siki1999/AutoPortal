using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoPortal.Models
{
    public class KorisnikPrijava
    {
        [Display(Name = "Korisničko ime")]
        [Required(ErrorMessage = "{0} je obavezno")]
        public string KorisnickoIme { get; set; }

        [Display(Name = "Lozinka")]
        [Required(ErrorMessage = "{0} je obavezna")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AutoPortal.Models
{
    [Table("korisnici")]
    public class Korisnik
    {
        [Key]
        [Column("korisnicko_ime")]
        [Display(Name = "Korisničko ime")]
        [Required(ErrorMessage = "{0} je obavezno")]
        public string KorisnickoIme { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} je obavezan")]
        [EmailAddress]
        public string Email { get; set; }

        public string Lozinka { get; set; }

        public string Salt { get; set; }

        [Display(Name = "Prezime")]
        [Required(ErrorMessage = "{0} je obavezno")]
        public string Prezime { get; set; }

        [Display(Name = "Ime")]
        [Required(ErrorMessage = "{0} je obavezno")]
        public string Ime { get; set; }

        public string PrezimeIme
        {
            get
            {
                return Prezime + " " + Ime;
            }
        }

        [Column("ovlast")]
        [Display(Name = "Ovlast")]
        [Required]
        [ForeignKey("Ovlast")]
        public string SifraOvlasti { get; set; }

        [Display(Name = "Ovlast")]
        public virtual Ovlast Ovlast { get; set; }

        [Display(Name = "Lozinka")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} je obavezna")]
        [NotMapped]
        public string LozinkaUnos { get; set; }

        [Display(Name = "Lozinka ponovljena")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} je obavezna")]
        [NotMapped]
        [Compare("LozinkaUnos", ErrorMessage = "Lozinke moraju biti jednake")]
        public string LozinkaUnos2 { get; set; }
    }
}
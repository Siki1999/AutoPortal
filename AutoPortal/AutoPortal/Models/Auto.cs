using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AutoPortal.Models
{
    [Table("auto")]
    public class Auto
    {
        [Key]
        [Display(Name = "ID auta")]
        public int IdAuto { get; set; }

        [Display(Name = "Marka")]
        [Required(ErrorMessage = "{0} je obavezna")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "{0} mora biti duljine minimalno {2} a maksimalno {1} znakova")]
        public string Marka { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "{0} je obavezan")]
        [StringLength(225, MinimumLength = 2, ErrorMessage = "{0} mora biti duljine minimalno {2} a maksimalno {1} znakova")]
        public string Model { get; set; }

        [Display(Name = "Obujam")]
        [Required(ErrorMessage = "{0} je obavezan")]
        public int Obujam { get; set; }

        [Display(Name = "Snaga")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public int Snaga { get; set; }

        [Display(Name = "Kilaža")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public int Kilaza { get; set; }

        [Display(Name = "Max Brzina")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public int Max_Brzina { get; set; }

        [Display(Name = "Ubrzanje")]
        [Required(ErrorMessage = "{0} je obavezno")]
        public float Ubrzanje { get; set; }

        [Display(Name = "Cijena")]
        [Required(ErrorMessage = "{0} je obavezna")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} mora biti duljine minimalno {2} a maksimalno {1} znakova")]
        public string Cijena { get; set; }
    }
}
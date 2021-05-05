using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPortal.Models
{
    [Table("ugovor")]
    public class Ugovor
    {
        [Key]
        [Display(Name = "ID Sastanka")]
        public int IdUgovor { get; set; }

        [Display(Name = "Email korisnika")]
        [Required(ErrorMessage = "{0} je obavezan")]
        [EmailAddress]
        public string Mail_Ug { get; set; }

        [Display(Name = "Model auta")]
        [Required(ErrorMessage = "{0} je obavezan")]
        public string Model { get; set; }

        [Display(Name = "Datum Sastanka")]
        [Required(ErrorMessage = "{0} je obavezan")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [Display(Name = "Vrijeme OD")]
        [Required(ErrorMessage = "{0} je obavezno")]
        [StringLength(5, ErrorMessage = "{0} mora biti u formatu (HH:mm)")]
        public string Vrijeme_Od { get; set; }

        [Display(Name = "Vrijeme DO")]
        [Required(ErrorMessage = "{0} je obavezno")]
        [StringLength(5, ErrorMessage = "{0} mora biti u formatu (HH:mm)")]
        public string Vrijeme_Do { get; set; }
    }
}
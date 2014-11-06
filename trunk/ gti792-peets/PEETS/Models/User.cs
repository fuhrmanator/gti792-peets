using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PEETS.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Courriel")]
        [StringLength(100, ErrorMessage = "La remarque doit être moins de 50 caractères")]
        [EmailAddress(ErrorMessage = "Courriel invalide")]
        public string Email { get; set; }

        [Display(Name = "Téléphone")]
        [Phone(ErrorMessage = "Téléphone invalide")]
        [StringLength(30, ErrorMessage = "La remarque doit être moins de 50 caractères")]
        public string PhoneNumber { get; set; }

        public string NomUtil { get; set; }
        public Profil UserProfil { get; set; }
    }
}
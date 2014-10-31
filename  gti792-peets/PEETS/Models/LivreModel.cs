using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace PEETS.Models
{    
    public class LivreModel
    {
        public int NoLivre { get; set; }

        [Required(ErrorMessage = "Le code ISBN est requis")]
        [StringLength(13, ErrorMessage = "Le code ISBN doit être de 10 ou de 13 chiffres")]
        public string CodeIsbn { get; set; }
        public string Image { get; set; }
        public string NomLivre { get; set; }
    }
}
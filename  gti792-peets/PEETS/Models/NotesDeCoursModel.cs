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
    public class NotesDeCoursModel
    {
        public int NoNotesDeCours { get; set; }

        [Required(ErrorMessage = "Le code barre est requis")]
        [StringLength(13, ErrorMessage = "Le code ISBN doit être de 12 chiffres")]
        public string CodeBarre { get; set; }
        public string Nom { get; set; }
        public string SousTitre { get; set; }
        public string MoisRedaction { get; set; }
        public int AnneeRedaction { get; set; }
        public string MoisRevision { get; set; }
        public int AnneeRevision { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEETS.Models
{
    public class NotesDeCoursBean
    {
        public int NoNotesDeCours { get; set; }
        public String Nom { get; set; }
        public String SousTitre { get; set; }
        public String CodeBarre { get; set; }
        public String MoisRedaction { get; set; }
        public int AnneeRedaction { get; set; }
        public String MoisRevision { get; set; }
        public int AnneeRevision { get; set; }
    }
}
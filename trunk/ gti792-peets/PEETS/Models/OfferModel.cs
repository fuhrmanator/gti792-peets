using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Windows.Forms;
using PEETS.Enums;

namespace PEETS.Models
{
    public class OfferModel
    {
        public int NoOffre { get; set; }

        [StringLength(200, ErrorMessage = "La remarque doit être moins de 50 caractères")]
        public string Remarques { get; set; }

        public string CoursObligatoires { get; set; }
        public string CoursReferences { get; set; }
        public String SelectedEtat { get; set; }

        public IEnumerable<SelectListItem> Etats = new List<SelectListItem>
        {
            new SelectListItem  {Value = "NEUF", Text = "Livre neuf"}, 
            new SelectListItem  {Value = "COMNE", Text = "Comme neuf"}, 
            new SelectListItem  {Value = "BONET", Text = "Bon état"}, 
            new SelectListItem {Value = "SATIS", Text = "Satisfaisant"},
            new SelectListItem {Value = "MOYEN", Text = "Moyen"},
            new SelectListItem {Value = "MAUVA", Text = "Mauvais état"}
        };

        public LivreModel Livre { get; set; }

        public String Message { get; set; }

        public TypeMessage TypeMessage { get; set; }

        public List<Offre> ListeOffresUtil { get; set; }
    }

}
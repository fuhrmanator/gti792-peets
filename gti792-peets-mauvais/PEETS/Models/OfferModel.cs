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
        public string DétailsFermeture { get; set; }
        public string CoursObligatoires { get; set; }
        public string CoursReferences { get; set; }
        public string SelectedEtat { get; set; }
        public string SelectedEtatModif { get; set; }
        public string SelectedRaison { get; set; }

        public IEnumerable<SelectListItem> Etats = new List<SelectListItem>
        {
            new SelectListItem  {Value = "NEUF", Text = "Livre neuf"}, 
            new SelectListItem  {Value = "COMNE", Text = "Comme neuf"}, 
            new SelectListItem  {Value = "BONET", Text = "Bon état"}, 
            new SelectListItem {Value = "SATIS", Text = "Satisfaisant"},
            new SelectListItem {Value = "MOYEN", Text = "Moyen"},
            new SelectListItem {Value = "MAUVA", Text = "Mauvais état"}
        };


        public IEnumerable<SelectListItem> ListeRaisons()
        {
            return ObtenirListeRaison().Select(raison => new SelectListItem
            {
                Value = raison.CodeRaison,
                Text = raison.DescRaison,
                Selected = raison.CodeRaison == "ECHAN"
            }).ToList();
        }


        public List<Raison> ObtenirListeRaison()
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var raisons = new List<Raison>();          
            cnn = new SqlConnection(connetionString);

            const string sql = "SELECT * FROM Raison r ";
            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                
                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    var raison = new Raison
                    {
                        CodeRaison = dataReader.GetValue(0).ToString(),
                        DescRaison = dataReader.GetValue(1).ToString()
                    };

                    raisons.Add(raison);
                }

                dataReader.Close();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Can not open connection ! ");
            }

            return raisons;
        }

        public LivreModel Livre { get; set; }

        public String Message { get; set; }

        public TypeMessage TypeMessage { get; set; }

        public List<OffreBean> ListeOffresUtil { get; set; }

        public Double Prix { get; set; }
    }

}

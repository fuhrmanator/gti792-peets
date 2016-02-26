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
    public class CalculatriceModel
    {
        public int NoCalculatrice { get; set; }

        [Required(ErrorMessage = "Le modèle est requis")]
        public string Modele { get; set; }

        public List<CalculatriceBean> ObtenirListeCalculatrice()
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var calculatrices = new List<CalculatriceBean>();
            cnn = new SqlConnection(connetionString);

            const string sql = "SELECT * FROM Calculatrice c ";
            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);

                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    var calcu = new CalculatriceBean
                    {
                        NoCalculatrice = int.Parse(dataReader.GetValue(0).ToString()),
                        Modele = dataReader.GetValue(1).ToString()
                    };

                    calculatrices.Add(calcu);
                }

                dataReader.Close();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Can not open connection ! ");
            }

            return calculatrices;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Windows.Forms;
using Facebook;
using Microsoft.Owin.Security.Facebook;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PEETS.Enums;
using PEETS.Models;

namespace PEETS.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View("/Views/Admin/PrintEtiquette.cshtml");
        }


        public ActionResult ImprimerEtiquette(bool pForBook, bool pForNotes, bool pForCalcu, DateTime pDateDebut, DateTime pDateFin)
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            cnn = new SqlConnection(connetionString);
            String path = @"C:\Users\Patrick\Desktop\";

            if(pForBook)
            {
                var totalOffre = 0;
                cnn.Open();
                string sql = "SELECT COUNT(Id) " +  
                             "FROM Offre "+ 
                             "WHERE IdTypeArticle = 1 AND DateCreation BETWEEN '" + pDateDebut.ToString() + "' AND '" + pDateFin +"'";
                var command = new SqlCommand(sql, cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    totalOffre = (int)dataReader.GetValue(0);
                }

                dataReader.Close();

                if(totalOffre > 0)
                {
                    string sqlEtiquetteLivres = "SELECT o.Id,l.Nom, l.AnneeEdition,o.CoursOblig, o.Prix " +
                                                "FROM Offre o " +
                                                "INNER JOIN Livre l ON l.Id = o.IdArticle AND o.IdTypeArticle = 1 " +
                                                "WHERE  DateCreation BETWEEN '" + pDateDebut.ToString() + "' AND '" + pDateFin + "'";
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand command2 = new SqlCommand(sqlEtiquetteLivres, cnn);
                    da.SelectCommand = command2;
                    DataSet dsEtiquetteLivres = new DataSet();

                    da.Fill(dsEtiquetteLivres);
                    DataTable dtEtiquetteLivres = dsEtiquetteLivres.Tables[0];

                    using (StreamWriter writer = System.IO.File.CreateText(path+"EtiquettesLivres.csv")) 
                    {
                        foreach(DataRow row in dtEtiquetteLivres.Rows)
                        {
                            writer.WriteLine(row[0]+", " + row[1] + ", " +row[2] + 
                                ", " + row[3] + ", " + row[4] +"$" );
                        }
                    }
                }

                cnn.Close();
            }

            if (pForNotes)
            {
                var totalOffreNotes = 0;
                cnn.Open();
                string sql = "SELECT COUNT(Id) " +
                             "FROM Offre " +
                             "WHERE IdTypeArticle = 2 AND DateCreation BETWEEN '" + pDateDebut.ToString() + "' AND '" + pDateFin + "'";
                var command = new SqlCommand(sql, cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    totalOffreNotes = (int)dataReader.GetValue(0);
                }

                dataReader.Close();

                if (totalOffreNotes > 0)
                {
                    string sqlEtiquetteNotes = "SELECT o.Id,n.Nom, n.SousTitre,o.CoursOblig, o.Prix " +
                                                "FROM Offre o " +
                                                "INNER JOIN NotesDeCours n ON n.IdNotesDeCours = o.IdArticle AND o.IdTypeArticle = 2 " +
                                                "WHERE  DateCreation BETWEEN '" + pDateDebut.ToString() + "' AND '" + pDateFin + "'";
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand command2 = new SqlCommand(sqlEtiquetteNotes, cnn);
                    da.SelectCommand = command2;
                    DataSet dsEtiquetteNotes = new DataSet();

                    da.Fill(dsEtiquetteNotes);
                    DataTable dtEtiquetteNotes = dsEtiquetteNotes.Tables[0];

                    using (StreamWriter writer = System.IO.File.CreateText(path+"EtiquettesNotes.csv"))
                    {
                        foreach (DataRow row in dtEtiquetteNotes.Rows)
                        {
                            writer.WriteLine(row[0]+"," + row[1] + ", " + row[2] +
                                ", " + row[3] + ", " + row[4] + "$");
                        }
                    }
                }

                cnn.Close();
            }

            if (pForCalcu)
            {
                var totalOffreCalcu = 0;
                cnn.Open();
                string sql = "SELECT COUNT(Id) " +
                             "FROM Offre " +
                             "WHERE IdTypeArticle = 3 AND DateCreation BETWEEN '" + pDateDebut.ToString() + "' AND '" + pDateFin + "'";
                var command = new SqlCommand(sql, cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    totalOffreCalcu = (int)dataReader.GetValue(0);
                }

                dataReader.Close();

                if (totalOffreCalcu > 0)
                {
                    string sqlEtiquetteCalcu = "SELECT o.Id,c.Modele, o.CoursOblig, o.Prix " +
                                                "FROM Offre o " +
                                                "INNER JOIN Calculatrice c ON c.IdCalculatrice = o.IdArticle AND o.IdTypeArticle = 3 " +
                                                "WHERE  DateCreation BETWEEN '" + pDateDebut.ToString() + "' AND '" + pDateFin + "'";
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand command2 = new SqlCommand(sqlEtiquetteCalcu, cnn);
                    da.SelectCommand = command2;
                    DataSet dsEtiquetteCalcu = new DataSet();

                    da.Fill(dsEtiquetteCalcu);
                    DataTable dtEtiquetteCalcu = dsEtiquetteCalcu.Tables[0];

                    using (StreamWriter writer = System.IO.File.CreateText(path+"EtiquettesCalculatrices.csv"))
                    {
                        foreach (DataRow row in dtEtiquetteCalcu.Rows)
                        {
                            writer.WriteLine(row[0] + ", " + row[1] + 
                                ", " + row[2] + ", " + row[3] +"$");
                        }
                    }
                }

                cnn.Close();
            }

          

            return View("/Views/Admin/PrintEtiquette.cshtml");
        }
	}
}
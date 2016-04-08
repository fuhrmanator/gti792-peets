using System;
using System.Configuration;
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

        public void SetDateForPrint(string pDateDebut, string pDateFin)
        {
            Session["DateDebut"] = pDateDebut;
            Session["DateFin"] = pDateFin;
        }

       
        

        public void ImprimerEtiquetteLivre()
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            cnn = new SqlConnection(connetionString);


                var totalOffre = 0;
                string dateDebut = Session["DateDebut"].ToString();
                string dateFin = Session["DateFin"].ToString();
                cnn.Open();
                string sql = "SELECT COUNT(Id) " +
                             "FROM Offre " +
                             "WHERE IdTypeArticle = 1 AND IndActif = 1 AND DateCreation BETWEEN '" + dateDebut + "' AND '" + dateFin + "'";
                var command = new SqlCommand(sql, cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    totalOffre = (int)dataReader.GetValue(0);
                }

                dataReader.Close();

                if (totalOffre > 0)
                {
                    string sqlEtiquetteLivres = "SELECT o.Id,l.Nom, l.AnneeEdition,o.CoursOblig, o.Prix " +
                                                "FROM Offre o " +
                                                "INNER JOIN Livre l ON l.Id = o.IdArticle AND o.IdTypeArticle = 1 " +
                                                "WHERE IndActif = 1 AND DateCreation BETWEEN '"+ dateDebut + "' AND '"+ dateFin +"'";
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand command2 = new SqlCommand(sqlEtiquetteLivres, cnn);
                    da.SelectCommand = command2;
                    DataSet dsEtiquetteLivres = new DataSet();

                    da.Fill(dsEtiquetteLivres);
                    DataTable dtEtiquetteLivres = dsEtiquetteLivres.Tables[0];



                    var sb = new StringBuilder();
                    foreach (DataRow row in dtEtiquetteLivres.Rows)
                    {
                        sb.AppendLine(row[0] + ", " + row[1] + ", " + row[2] +
                            ", " + row[3] + ", " + row[4] + "$");
                    }
                    byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
                    string filename = "EtiquettesLivres.csv";
                    if (bytes != null)
                    {
                        Response.Clear();
                        Response.ContentType = "text/csv";
                        Response.AddHeader("Content-disposition", "attachment; filename=" + filename);
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.BinaryWrite(bytes);
                        Response.End();
                    }

              
                }

                cnn.Close();

        }

        public void ImprimerEtiquetteNotesDeCours()
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            cnn = new SqlConnection(connetionString);


            var totalOffre = 0;
            string dateDebut = Session["DateDebut"].ToString();
            string dateFin = Session["DateFin"].ToString();
            cnn.Open();
            string sql = "SELECT COUNT(Id) " +
                         "FROM Offre " +
                         "WHERE IdTypeArticle = 2 AND IndActif = 1 AND DateCreation BETWEEN '" + dateDebut + "' AND '" + dateFin + "'";
            var command = new SqlCommand(sql, cnn);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                totalOffre = (int)dataReader.GetValue(0);
            }

            dataReader.Close();

            if (totalOffre > 0)
            {
                string sqlEtiquetteNotes = "SELECT o.Id,n.Nom, n.SousTitre,o.CoursOblig, o.Prix " +
                                                "FROM Offre o " +
                                                "INNER JOIN NotesDeCours n ON n.IdNotesDeCours = o.IdArticle AND o.IdTypeArticle = 2 " +
                                                "WHERE IndActif = 1 AND  DateCreation BETWEEN '" + dateDebut + "' AND '" + dateFin + "'";
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand command2 = new SqlCommand(sqlEtiquetteNotes, cnn);
                da.SelectCommand = command2;
                DataSet dsEtiquetteNotes = new DataSet();

                da.Fill(dsEtiquetteNotes);
                DataTable dtEtiquetteNotes = dsEtiquetteNotes.Tables[0];

                var sb = new StringBuilder();
                foreach (DataRow row in dtEtiquetteNotes.Rows)
                {
                    sb.AppendLine(row[0] + "," + row[1] + ", " + row[2] +
                        ", " + row[3] + ", " + row[4] + "$");
                }
                byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
                string filename = "EtiquettesNotesDeCours.csv";
                if (bytes != null)
                {
                    Response.Clear();
                    Response.ContentType = "text/csv";
                    Response.AddHeader("Content-disposition", "attachment; filename=" + filename);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.End();
                }


            }

            cnn.Close();

        }

        public void ImprimerEtiquetteCalculatrices()
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            cnn = new SqlConnection(connetionString);


            var totalOffre = 0;
            string dateDebut = Session["DateDebut"].ToString();
            string dateFin = Session["DateFin"].ToString();
            cnn.Open();
            string sql = "SELECT COUNT(Id) " +
                         "FROM Offre " +
                         "WHERE IdTypeArticle = 3 AND IndActif = 1 AND DateCreation BETWEEN '" + dateDebut + "' AND '" + dateFin + "'";
            var command = new SqlCommand(sql, cnn);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                totalOffre = (int)dataReader.GetValue(0);
            }

            dataReader.Close();

            if (totalOffre > 0)
            {
                string sqlEtiquetteCalcu = "SELECT o.Id,c.Modele, o.CoursOblig, o.Prix " +
                                                  "FROM Offre o " +
                                                  "INNER JOIN Calculatrice c ON c.IdCalculatrice = o.IdArticle AND o.IdTypeArticle = 3 " +
                                                  "WHERE IndActif = 1 AND DateCreation BETWEEN '" + dateDebut + "' AND '" + dateFin + "'";
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand command2 = new SqlCommand(sqlEtiquetteCalcu, cnn);
                da.SelectCommand = command2;
                DataSet dsEtiquetteCalcu = new DataSet();

                da.Fill(dsEtiquetteCalcu);
                DataTable dtEtiquetteCalcu = dsEtiquetteCalcu.Tables[0];

                var sb = new StringBuilder();
                foreach (DataRow row in dtEtiquetteCalcu.Rows)
                {
                    sb.AppendLine(row[0] + ", " + row[1] +
                        ", " + row[2] + ", " + row[3] + "$");
                }
                byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
                string filename = "EtiquettesCalculatrice.csv";
                if (bytes != null)
                {
                    Response.Clear();
                    Response.ContentType = "text/csv";
                    Response.AddHeader("Content-disposition", "attachment; filename=" + filename);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.End();
                }


            }

            cnn.Close();

        }
	}
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using ComputerBeacon.Facebook.Fql;
using OpenLibrary.Extension;
using PEETS.Models;

namespace PEETS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PageActuel = 1;
            return View(ObtenirListeLivres());
        }

        public ActionResult ObtenirListeLivresParPage(string page, int pageActuel, int pageTotal)
        {
            pageTotal = Offre.GetTotalRows()/8;
            var count = pageTotal / 8.0;
            var pageCount = (int)Math.Ceiling((decimal)count);
            var newPageActuel = GererPage(page, pageActuel, pageCount);
            int start = ((newPageActuel - 1) * 8) + 1;
            int last = start + 7;
            ViewBag.PageActuel = newPageActuel;

            return View("Index", ObtenirListeLivres(start, last));
        }

        private int GererPage(string page, int pageActuel, int pageTotal)
        {
            int n;
            bool isNumeric = int.TryParse(page, out n);
            int pageNumber = 1;

            if (isNumeric)
            {
                pageNumber = n;
            }
            else if (page == "+>1")
            {
                if (pageActuel < pageTotal)
                {
                    pageNumber = pageActuel + 1;
                }
                else if (pageActuel > pageTotal)
                {
                    pageNumber = 1;
                }
                else
                    pageNumber = pageActuel;
            }
            else if (page == "<-1")
            {
                if (pageActuel > 1)
                {
                    pageNumber = pageActuel - 1;
                }
                else if (pageActuel < 1)
                {
                    pageNumber = 1;
                }
                else
                    pageNumber = pageActuel;
            }

            return pageNumber;
        }

        public List<Offre> ObtenirListeLivres(int start = 1, int last = 8, string reqRech = "")
        {
            List<Offre> offres = null;
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            string sql = "SELECT a.* FROM(";
            sql += "SELECT o.Id, e.DesctEtat, o.CoursOblig, o.CoursRecom, l.CodeISBN_10, " +
                   "l.CodeISBN_13, l.Nom, l.Image, o.Remarques, l.SousTitre, l.Auteur, o.IndActif, " +
                   "ROW_NUMBER() OVER (ORDER BY  l.Nom ASC) AS ROWNUMBERS " +
                         "FROM Offre o " +
                         "JOIN Livre l On o.IdLivre = l.Id " +
                         "JOIN Etat e ON o.Etat = e.CodeEtat " +
                         "Where o.IndActif='1' " + reqRech;

            sql += ") a WHERE a.ROWNUMBERS BETWEEN " + start + " AND " + last;
         
            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                command = new SqlCommand(sql, cnn);
                offres = new List<Offre>();

                SqlDataReader dataReader = command.ExecuteReader();
          
                while (dataReader.Read())
                {
                    var offre = new Offre
                    {
                        NoOffre = (int) dataReader.GetValue(0),
                        EtatLivre = dataReader.GetValue(1).ToString(),
                        CoursObligatoires = dataReader.GetValue(2).ToString(),
                        CoursRecommandes = dataReader.GetValue(3).ToString(),
                        CodeIsbn_10 = dataReader.GetValue(4).ToString(),
                        CodeIsbn_13 = dataReader.GetValue(5).ToString(),
                        NomLivre = dataReader.GetValue(6).ToString(),
                        ImageLivre = dataReader.GetValue(7).ToString() != "" ? dataReader.GetValue(7).ToString() : "https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcR7lSOcD29L4UQ8vuBd3rj1CREOKYOxCQE1Qrf8rAJmC500pR_9dA",
                        Remarques = dataReader.GetValue(8).ToString(),
                        SousTitre = dataReader.GetValue(9).ToString(),
                        Auteur = dataReader.GetValue(10).ToString()
                    };                   

                    offres.Add(offre);
                }

                dataReader.Close();      
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }

            return offres;
        }

        public ActionResult Rechercher(string titre, string isbn, string auteur, string sigle, int pageActuel)
        {
            var totalRows = Offre.GetTotalRows();
            var count = totalRows / 8.0;
            var pageCount = (int)Math.Ceiling((decimal)count);
            var newPageActuel = GererPage("", pageActuel, pageCount);
            int start = ((newPageActuel - 1) * 8) + 1;
            int last = start + 7;
            string recherche = "";

            if (!string.IsNullOrEmpty(titre))
            {
                recherche += " And l.Nom like '%" + titre + "%'";
            }
            else if (!string.IsNullOrEmpty(isbn))
            {
                recherche += " And (l.CodeISBN_10 like '%" + isbn + "%' Or l.CodeISBN_13 like '%" + isbn + "%')";
            }
           else if (!string.IsNullOrEmpty(sigle))
           {
               recherche += " And (o.CoursOblig like '%" + sigle + "%' Or o.CoursRecom like '%" + sigle + "%')";
           }
            else if (!string.IsNullOrEmpty(auteur))
           {
               recherche += " And l.Auteur like '%" + auteur + "%'";
           }

            ViewBag.PageActuel = 1;

            return View("AffichageLivre", ObtenirListeLivres(start, last, recherche));
             
        }
        public ActionResult GetDetailsJson(int noOffre)
        {
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            Offre offre = null;
            string sql = "SELECT o.Id, e.DesctEtat, o.CoursOblig, o.CoursRecom, l.CodeISBN_10, " +
                         "l.CodeISBN_13, l.Nom, l.Image, o.Remarques, u.Email, u.PhoneNumber, l.Auteur " +
                         "FROM Offre o " +
                         "JOIN Livre l On o.IdLivre = l.Id " +
                         "JOIN Etat e ON o.Etat = e.CodeEtat " +
                         "JOIN AspNetUsers u On u.Id = o.userId " +
                         "Where o.Id = " + noOffre;

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                command = new SqlCommand(sql, cnn);
                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    offre = new Offre
                    {
                        NoOffre = (int)dataReader.GetValue(0),
                        EtatLivre = dataReader.GetValue(1).ToString(),
                        CoursObligatoires = dataReader.GetValue(2).ToString(),
                        CoursRecommandes = dataReader.GetValue(3).ToString(),
                        CodeIsbn_10 = dataReader.GetValue(4).ToString(),
                        CodeIsbn_13 = dataReader.GetValue(5).ToString(),
                        NomLivre = dataReader.GetValue(6).ToString(),
                        ImageLivre = dataReader.GetValue(7).ToString(),
                        Remarques = dataReader.GetValue(8).ToString(),
                        Email = dataReader.GetValue(9).ToString(),
                        Phone = dataReader.GetValue(10).ToString(),
                        Auteur = dataReader.GetValue(11).ToString()
                    };

                }

                dataReader.Close();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                offre = new Offre();
                offre.Message = "Une erreur est survenue";
            }

            return Json(offre, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
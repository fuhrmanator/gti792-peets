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
            ViewBag.menuItemActive = "Index";
            return View(ObtenirListeLivres());
        }

        public ActionResult ObtenirListeLivresParPage(string pageDemande, int pageActuel, string titre, string isbn, string auteur, string sigle, string tri = "l.Nom", string ordre = "ASC")
        {
            string recherche = construireRecherche(titre, isbn, auteur, sigle);
            var pageTotal = OffreBean.GetTotalRows(recherche);
            var count = pageTotal / 8.0;
            var pageCount = (int)Math.Ceiling((decimal)count);
            var newPageActuel = GererPage(pageDemande, pageActuel, pageCount);
            int start = ((newPageActuel - 1) * 8) + 1;
            int last = start + 7;
            ViewBag.PageActuel = newPageActuel;
            ViewBag.ReqRech = recherche;
            return View("Index", ObtenirListeLivres(start, last, recherche,tri,ordre));
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
            else switch (page)
            {
                case "+>1":
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
                    break;
                case "<-1":
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
                    break;
            }

            return pageNumber;
        }

        public List<OffreBean> ObtenirListeLivres(int start = 1, int last = 8, string reqRech = "", string tri = "l.Nom", string ordre = "ASC")
        {
            List<OffreBean> offres = null;
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;

            if (string.IsNullOrEmpty(tri))
            {
                tri = "l.Nom";
                ordre = "ASC";
            }

            var sql = "SELECT a.* FROM(";
            sql += "SELECT o.Id, e.DesctEtat, o.CoursOblig, o.CoursRecom, l.CodeISBN_10, " +
                   "l.CodeISBN_13, l.Nom, l.Image, o.Remarques, l.SousTitre, l.Auteur,o.Prix, o.IndActif, " +
                   "ROW_NUMBER() OVER (ORDER BY " + tri + " " + ordre + " ) AS ROWNUMBERS " +
                         "FROM Offre o " +
                         "JOIN Livre l On o.IdArticle = l.Id  " +
                         "JOIN Etat e ON o.Etat = e.CodeEtat " +
                         "Where o.IndActif='1' AND o.IdTypeArticle = 1" + reqRech;

            sql += ") a WHERE a.ROWNUMBERS BETWEEN " + start + " AND " + last;
         
            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                offres = new List<OffreBean>();

                SqlDataReader dataReader = command.ExecuteReader();
          
                while (dataReader.Read())
                {
                    var offre = new OffreBean
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
                        Auteur = dataReader.GetValue(10).ToString(),
                        Prix = Convert.ToDouble(dataReader.GetValue(11).ToString())
                    };                   

                    offres.Add(offre);
                }

                dataReader.Close();      
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                
            }

            if (offres != null && offres.Count > 0)
            {
                offres.First().OrdreItems.First(x => x.Value.Equals(ordre)).Selected = true;
                offres.First().TriItems.First(x => x.Value.Equals(tri)).Selected = true;
            }
            return offres;
        }

        public ActionResult Trier(string tri, string ordre, int pageActuel, string titre, string isbn, string auteur, string sigle)
        {
            string recherche = construireRecherche(titre, isbn, auteur, sigle);
            var totalRows = OffreBean.GetTotalRows(recherche);
            var count = totalRows / 8.0;
            var pageCount = (int)Math.Ceiling((decimal)count);
            var newPageActuel = GererPage("", pageActuel, pageCount);
            int start = ((newPageActuel - 1) * 8) + 1;
            int last = start + 7;
            ViewBag.PageActuel = newPageActuel;            
            ViewBag.ReqRech = recherche;
            return View("Index", ObtenirListeLivres(start, last, recherche, tri, ordre));
        }
        public ActionResult Rechercher(string titre, string isbn, string auteur, string sigle, int pageActuel, string tri, string ordre)
        {
            string recherche = construireRecherche(titre, isbn, auteur, sigle); 
            var totalRows = OffreBean.GetTotalRows(recherche);
            var count = totalRows / 8.0;
            var pageCount = (int)Math.Ceiling((decimal)count);
            var newPageActuel = GererPage("", pageActuel, pageCount);
            int start = ((newPageActuel - 1) * 8) + 1;
            int last = start + 7;                     
            ViewBag.PageActuel = 1;
            ViewBag.ReqRech = recherche;
            return View("Index", ObtenirListeLivres(start, last, recherche, tri, ordre));
             
        }

        private string construireRecherche(string titre, string isbn, string auteur, string sigle)
        {
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

            return recherche;
        }

        public ActionResult GetDetailsJson(int noOffre)
        {
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            OffreBean offre = null;
            string sql = "SELECT o.Id, e.DesctEtat, o.CoursOblig, o.CoursRecom, l.CodeISBN_10, " +
                         "l.CodeISBN_13, l.Nom, l.Image, o.Remarques, u.Email, u.PhoneNumber, l.Auteur, o.Prix, l.AnneeEdition " +
                         "FROM Offre o " +
                         "JOIN Livre l On o.IdArticle = l.Id " +
                         "JOIN Etat e ON o.Etat = e.CodeEtat " +
                         "JOIN AspNetUsers u On u.Id = o.userId " +
                         "Where o.IdTypeArticle = 1 AND o.Id = " + noOffre;

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                command = new SqlCommand(sql, cnn);
                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    offre = new OffreBean
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
                        Auteur = dataReader.GetValue(11).ToString(),
                        Prix = Convert.ToDouble(dataReader.GetValue(12).ToString()),
                        AnneeEdition = dataReader.GetValue(13).ToString()
                    };

                }

                dataReader.Close();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                offre = new OffreBean();
                offre.Message = "Une erreur est survenue";
            }

            return Json(offre, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.menuItemActive = "About";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.menuItemActive = "Contact";
            return View();
        }
    }
}
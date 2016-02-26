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
using System.Windows.Forms;
using Facebook;
using Microsoft.Owin.Security.Facebook;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PEETS.Enums;
using PEETS.Models;

namespace PEETS.Controllers
{
    public class OfferController : Controller
    {         
        [WebMethod]
        public ActionResult Index()
        {
            var offreModel = new OfferModel { Livre = new LivreModel { }, ListeOffresUtil = ObtenirListeOffresUtil(), ListeOffresUtilNotesCours = ObtenirListeOffresUtilNotesCours(), ListeOffresUtilCalculatrices = ObtenirListeOffresUtilCalculatrice() };
            ViewBag.menuItemActive = "Offre";
            return View("ManageOffer", offreModel);
        }

        public List<OffreBean> ObtenirListeOffresUtil()
        {
            List<OffreBean> offres = null;
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var sql = "SELECT o.Id, l.Nom " +
                         "FROM Offre o " +
                         "JOIN Livre l On o.IdArticle = l.Id " +
                         "Where o.IdTypeArticle = 1 AND o.userId = '" + User.Identity.GetUserId() + "' And o.IndActif='1' order by o.Id desc";

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                offres = new List<OffreBean>();

                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    var offre = new OffreBean
                    {
                        NoOffre = (int)dataReader.GetValue(0),
                        NomLivre = dataReader.GetValue(1).ToString(),
                        estNouv = false
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

            return offres;
        }

        public List<OffreBean> ObtenirListeOffresUtilNotesCours()
        {
            List<OffreBean> offres = null;
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var sql = "SELECT o.Id, n.Nom " +
                         "FROM Offre o " +
                         "JOIN NotesDeCours n On o.IdArticle = n.IdNotesDeCours " +
                         "Where o.IdTypeArticle = 2 AND o.userId = '" + User.Identity.GetUserId() + "' And o.IndActif='1' order by o.Id desc";

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                offres = new List<OffreBean>();

                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    var offre = new OffreBean
                    {
                        NoOffre = (int)dataReader.GetValue(0),
                        NomLivre = dataReader.GetValue(1).ToString(),
                        estNouv = false
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

            return offres;
        }

        public List<OffreBean> ObtenirListeOffresUtilCalculatrice()
        {
            List<OffreBean> offres = null;
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var sql = "SELECT o.Id, c.Modele " +
                         "FROM Offre o " +
                         "JOIN Calculatrice c On o.IdArticle = c.IdCalculatrice " +
                         "Where o.IdTypeArticle = 3 AND o.userId = '" + User.Identity.GetUserId() + "' And o.IndActif='1' order by o.Id desc";

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                offres = new List<OffreBean>();

                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    var offre = new OffreBean
                    {
                        NoOffre = (int)dataReader.GetValue(0),
                        ModeleCalculatrice = dataReader.GetValue(1).ToString(),
                        estNouv = false
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

            return offres;
        }

        public ActionResult Create(OfferModel offre)
        {
                if (offre != null)
                {

                    switch(offre.TypeArticle)
                    {
                        case 1:
                        {
                            int? noLivre = TraiterLivre(offre.Livre);
                            int? id = null;

                            if (noLivre != null)
                            {                      
                                offre.Livre.NoLivre = (int)noLivre;
  
                                var connetionString = Properties.Settings.Default.dbConnectionString;
                                string sql = "INSERT INTO Offre(IdArticle, Remarques, Etat, CoursOblig, CoursRecom, userId, Prix, DateCreation, IdTypeArticle) OUTPUT Inserted.ID " +
                                                   "VALUES(@IdLivre, @Remarques, @Etat, @CoursOblig, @CoursRecom, @userId, @Prix, GETDATE(),1) SET @id=SCOPE_IDENTITY()";

                                var cnn = new SqlConnection(connetionString);

                                try
                                {
                                    cnn.Open();

                                    var command = new SqlCommand(sql, cnn);

                                    RemplirParametreOffre(command, offre);

                                    command.ExecuteNonQuery();
                                    id = (int?)command.Parameters["@id"].Value;
                                    command.Dispose();
                                    cnn.Close();

                                    //publishToFacebook();

                                    offre.Message = "L'offre " + id + " a été créée avec succès.";
                                    offre.TypeMessage = TypeMessage.Succes;
                                }
                                catch (Exception ex)
                                {
                                    offre.Message = "Un problème est survenu lors de la création de l'offre.";
                                    offre.TypeMessage = TypeMessage.Erreur;
                                }
                            }
                            else
                            {
                                offre.Message = "Le code ISBN soumis n'est pas valide.";
                                offre.TypeMessage = TypeMessage.Warning;
                            }

                            offre.ListeOffresUtilCalculatrices = ObtenirListeOffresUtilCalculatrice();
                            offre.ListeOffresUtilNotesCours = ObtenirListeOffresUtilNotesCours();
                            offre.ListeOffresUtil = ObtenirListeOffresUtil();
                            offre.ListeOffresUtil.First(x => x.NoOffre == id).estNouv = true;
                            offre.Livre = new LivreModel {};
                        }
                        break;
                        case 2 :

                            int? noNotesDeCours = TraiterNotesDeCours(offre.NotesDeCours);
                            int? idOffreNotesCours = null;

                            if (noNotesDeCours != null)
                            {

                                offre.NotesDeCours.NoNotesDeCours = (int)noNotesDeCours;
  
                                var connetionString = Properties.Settings.Default.dbConnectionString;
                                string sql = "INSERT INTO Offre(IdArticle, Remarques, Etat, CoursOblig, CoursRecom, userId, Prix, DateCreation, IdTypeArticle) OUTPUT Inserted.ID " +
                                                   "VALUES(@IdNotes, @Remarques, @Etat, @CoursOblig, @CoursRecom, @userId, @Prix, GETDATE(),2) SET @id=SCOPE_IDENTITY()";

                                var cnn = new SqlConnection(connetionString);

                                try
                                {
                                    cnn.Open();

                                    var command = new SqlCommand(sql, cnn);

                                    RemplirParametreOffre(command, offre);

                                    command.ExecuteNonQuery();
                                    idOffreNotesCours = (int?)command.Parameters["@id"].Value;
                                    command.Dispose();
                                    cnn.Close();

                                    //publishToFacebook();

                                    offre.Message = "L'offre " + idOffreNotesCours + " a été créée avec succès.";
                                    offre.TypeMessage = TypeMessage.Succes;
                                }
                                catch (Exception ex)
                                {
                                    offre.Message = "Un problème est survenu lors de la création de l'offre.";
                                    offre.TypeMessage = TypeMessage.Erreur;
                                }
                            }
                            else
                            {
                                offre.Message = "Le code barre soumis n'est pas valide.";
                                offre.TypeMessage = TypeMessage.Warning;
                            }

                            offre.ListeOffresUtilCalculatrices = ObtenirListeOffresUtilCalculatrice();
                            offre.ListeOffresUtilNotesCours = ObtenirListeOffresUtilNotesCours();
                            offre.ListeOffresUtil = ObtenirListeOffresUtil();
                            offre.ListeOffresUtilNotesCours.First(x => x.NoOffre == idOffreNotesCours).estNouv = true;
                            offre.NotesDeCours = new NotesDeCoursModel {};
                        break;
                        case 3:

                        int? noCalculatrice = offre.Calculatrice.NoCalculatrice;
                        int? idOffreCalculatrice = null;

                        if (noCalculatrice != 0)
                        {
                            offre.Calculatrice.NoCalculatrice = (int)noCalculatrice;

                            var connetionString = Properties.Settings.Default.dbConnectionString;
                            string sql = "INSERT INTO Offre(IdArticle, Remarques, Etat, CoursOblig, CoursRecom, userId, Prix, DateCreation, IdTypeArticle) OUTPUT Inserted.ID " +
                                               "VALUES(@IdCalculatrice, @Remarques, @Etat, @CoursOblig, @CoursRecom, @userId, @Prix, GETDATE(),3) SET @id=SCOPE_IDENTITY()";

                            var cnn = new SqlConnection(connetionString);

                            try
                            {
                                cnn.Open();

                                var command = new SqlCommand(sql, cnn);

                                RemplirParametreOffre(command, offre);

                                command.ExecuteNonQuery();
                                idOffreCalculatrice = (int?)command.Parameters["@id"].Value;
                                command.Dispose();
                                cnn.Close();

                                //publishToFacebook();

                                offre.Message = "L'offre " + idOffreCalculatrice + " a été créée avec succès.";
                                offre.TypeMessage = TypeMessage.Succes;
                            }
                            catch (Exception ex)
                            {
                                offre.Message = "Un problème est survenu lors de la création de l'offre.";
                                offre.TypeMessage = TypeMessage.Erreur;
                            }
                        }
                        else
                        {
                            offre.Message = "Le modèle soumis n'est pas valide.";
                            offre.TypeMessage = TypeMessage.Warning;
                        }

                        offre.ListeOffresUtilCalculatrices = ObtenirListeOffresUtilCalculatrice();
                        offre.ListeOffresUtilNotesCours = ObtenirListeOffresUtilNotesCours();
                        offre.ListeOffresUtil = ObtenirListeOffresUtil();
                        offre.ListeOffresUtilCalculatrices.First(x => x.NoOffre == idOffreCalculatrice).estNouv = true;
                        offre.Calculatrice = new CalculatriceModel { };
                        break;
                    }
                }

                ViewBag.menuItemActive = "Offre";
                return View("ManageOffer", offre);
        }

        public ActionResult Modifier(String noOffre, String coursOblig, String coursRecom, String etat, String rem, Double prix)
        {
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var sql = "Update Offre Set Remarques = '" + rem + "', CoursOblig = '" + coursOblig + "', CoursRecom = '" + coursRecom + "',Prix = " + prix.ToString() + ", Etat = '" + etat + "' Where Id = " + noOffre;
            var cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                command.ExecuteNonQuery();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailsJson(int noOffre)
        {
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            OffreBean offre = null;
            string sql = "SELECT o.Id, e.DesctEtat, o.CoursOblig, o.CoursRecom, l.CodeISBN_10, " +
                         "l.CodeISBN_13, l.Nom, l.Image, o.Remarques, u.Email, u.PhoneNumber, l.Auteur, o.Prix,l.AnneeEdition " +
                         "FROM Offre o " +
                         "JOIN Livre l On o.IdArticle = l.Id " +
                         "JOIN Etat e ON o.Etat = e.CodeEtat " +
                         "JOIN AspNetUsers u On u.Id = o.userId " +
                         "Where o.IdTypeArticle=1 AND o.Id = " + noOffre;

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

        public ActionResult GetDetailsJsonNotes(int noOffre)
        {
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            OffreBean offre = null;
            string sql = "SELECT o.Id, e.DesctEtat, o.CoursOblig, o.CoursRecom, " +
                         "n.Nom, n.SousTitre,n.MoisRedaction,n.AnneeRedaction,n.MoisRevision,n.AnneeRevision, o.Remarques, u.Email, u.PhoneNumber,  o.Prix " +
                         "FROM Offre o " +
                         "JOIN NotesDeCours n On o.IdArticle = n.IdNotesDeCours " +
                         "JOIN Etat e ON o.Etat = e.CodeEtat " +
                         "JOIN AspNetUsers u On u.Id = o.userId " +
                         "Where o.IdTypeArticle=2 AND o.Id = " + noOffre;

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
                        NomNotesCours = dataReader.GetValue(4).ToString(),
                        SousTitre = dataReader.GetValue(5).ToString(),
                        MoisRedaction = dataReader.GetValue(6).ToString(),
                        AnneeRedaction = int.Parse(dataReader.GetValue(7).ToString()),
                        MoisRevision = dataReader.GetValue(8).ToString(),
                        AnneeRevision = int.Parse(dataReader.GetValue(9).ToString()),
                        Remarques = dataReader.GetValue(10).ToString(),
                        Email = dataReader.GetValue(11).ToString(),
                        Phone = dataReader.GetValue(12).ToString(),
                        Prix = Convert.ToDouble(dataReader.GetValue(13).ToString()),
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

        public ActionResult GetDetailsJsonCalculatrice(int noOffre)
        {
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            OffreBean offre = null;
            string sql = "SELECT o.Id, e.DesctEtat, o.CoursOblig, o.CoursRecom, " +
                         "c.Modele, o.Remarques, u.Email, u.PhoneNumber,  o.Prix " +
                         "FROM Offre o " +
                         "JOIN Calculatrice c On o.IdArticle = c.IdCalculatrice " +
                         "JOIN Etat e ON o.Etat = e.CodeEtat " +
                         "JOIN AspNetUsers u On u.Id = o.userId " +
                         "Where o.IdTypeArticle=3 AND o.Id = " + noOffre;

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
                        ModeleCalculatrice = dataReader.GetValue(4).ToString(),
                        Remarques = dataReader.GetValue(5).ToString(),
                        Email = dataReader.GetValue(6).ToString(),
                        Phone = dataReader.GetValue(7).ToString(),
                        Prix = Convert.ToDouble(dataReader.GetValue(8).ToString()),
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

        private string Encoding(string texte)
        {
            texte = texte.Replace("Ã", "à");
            texte = texte.Replace("Ã©", "é");
            texte = texte.Replace("à®", "î");
            texte = texte.Replace("à©", "é");
            texte = texte.Replace("Ã¨", "è");
            texte = texte.Replace("à¨", "è");
            texte = texte.Replace("Ã‰", "É");
            texte = texte.Replace("à‰", "É");
            texte = texte.Replace("à§", "ç");
            texte = texte.Replace("Ãª", "ê");
            texte = texte.Replace("àª", "ê");
            texte = texte.Replace("Ã¢", "â");
            texte = texte.Replace("à¢", "â");
            return texte;
        }

        public ActionResult GetInfoLivreJson(String codeIsbn)
        {
            var url = "https://www.googleapis.com/books/v1/volumes?q=isbn:" + codeIsbn;
            var webClient = new System.Net.WebClient();
            var json = Encoding(webClient.DownloadString(url));

            var gRresponse = JsonConvert.DeserializeObject<GoogleResponse>(json);
            if (gRresponse.Items == null) return null;

            var volumeInfo = gRresponse.Items[0].VolumeInfo;
            if (volumeInfo == null) return null;

            var livre = new LivreBean();
            livre.NomLivre = volumeInfo.Title;
            livre.AnneeEdition = int.Parse(volumeInfo.PublishedDate.Substring(0, 4));

            if (volumeInfo.Authors != null)
            {
                livre.Auteur = "";

                foreach (var aut in volumeInfo.Authors.ToList())
                {
                    if (livre.Auteur != "")
                    {
                        livre.Auteur += " ; ";
                    }

                    livre.Auteur += aut;
                }
            }

            return Json(livre, JsonRequestBehavior.AllowGet);
        }

        public int? TraiterLivre(LivreModel livre)
        {
            if (livre == null) return null;
            var noLivreReturn = livreExiste(livre.CodeIsbn);
            if (noLivreReturn != null) return noLivreReturn;

            var url = "https://www.googleapis.com/books/v1/volumes?q=isbn:" + livre.CodeIsbn;
            var webClient = new System.Net.WebClient();
            var json = Encoding(webClient.DownloadString(url));  
             
            var gRresponse = JsonConvert.DeserializeObject<GoogleResponse>(json);
            if (gRresponse.Items == null) return null;

            var volumeInfo = gRresponse.Items[0].VolumeInfo;
            if (volumeInfo == null) return null;

            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            string sql = "INSERT INTO Livre(CodeIsbn_10, Nom, Image, Auteur, Image2, CodeIsbn_13, SousTitre,AnneeEdition) OUTPUT Inserted.ID " +
                         "VALUES(@CodeIsbn_10, @Nom, @Image, @Auteur, @Image2, @CodeIsbn_13, @SousTitre, @AnneeEdition) SET @id=SCOPE_IDENTITY()";

            cnn = new SqlConnection(connetionString);
            cnn.Open();
            var command = new SqlCommand(sql, cnn);

            RemplirParametreLivre(command, volumeInfo);
            command.ExecuteNonQuery();

            var id = (int?)command.Parameters["@id"].Value;
            noLivreReturn = id;
            command.Dispose();
            cnn.Close();

            return noLivreReturn;
        }

        public int? TraiterNotesDeCours(NotesDeCoursModel notesDeCours)
        {
            if (notesDeCours == null) return null;
            var noNotesReturn = NotesDeCoursExiste(notesDeCours.CodeBarre);
            if (noNotesReturn != null) return noNotesReturn;

    
            SqlConnection cnn = null;
            string connectionString = Properties.Settings.Default.dbConnectionString;
            string sql = "INSERT INTO NotesDeCours(Nom,SousTitre,MoisRedaction ,AnneeRedaction, MoisRevision,AnneeRevision, CodeBarre) OUTPUT Inserted.IdNotesDeCours " +
                         "VALUES(@Nom,@SousTitre, @MoisRedaction, @AnneeRedaction, @MoisRevision,@AnneeRevision,@CodeBarre) SET @id=SCOPE_IDENTITY()";

            cnn = new SqlConnection(connectionString);
            cnn.Open();
            var command = new SqlCommand(sql, cnn);

            RemplirParametreNotesDeCours(command, notesDeCours);
            command.ExecuteNonQuery();

            var id = (int?)command.Parameters["@id"].Value;
            noNotesReturn = id;
            command.Dispose();
            cnn.Close();

            return noNotesReturn;
        }

        private int? livreExiste(string codeIsbn)
        {
            int? noLivre = null;

            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            string sql = "SELECT l.Id FROM Livre l Where l.CodeISBN_10 = '" + codeIsbn + "' Or " + "l.CodeISBN_13 = '" + codeIsbn  + "'";

            cnn = new SqlConnection(connetionString);

            cnn.Open();
            command = new SqlCommand(sql, cnn);

            SqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {

                noLivre = (int)dataReader.GetValue(0);
            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

            return noLivre;

        }

        private int? NotesDeCoursExiste(string codeBarre)
        {
            int? noNotesDeCours = null;

            SqlConnection cnn = null;
            string connectionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            string sql = "SELECT n.IdNotesDeCours FROM NotesDeCours n Where n.CodeBarre = '" + codeBarre + "'";

            cnn = new SqlConnection(connectionString);

            cnn.Open();
            command = new SqlCommand(sql, cnn);

            SqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {

                noNotesDeCours = (int)dataReader.GetValue(0);
            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

            return noNotesDeCours;

        }
        public void RemplirParametreLivre(SqlCommand command, VolumeInfo volumeInfo)
        {
            var codeIsnb10 = "";
            var codeIsnb13 = "";
            var auteur = "";
            var anneeEdition = "";

            if(volumeInfo.PublishedDate != "")
            {
                anneeEdition = volumeInfo.PublishedDate.Substring(0,4);
            }
            if (volumeInfo.IndustryIdentifiers != null)
            {
                foreach (var code in volumeInfo.IndustryIdentifiers.ToList())
                {
                    if (code.Type == "ISBN_10")
                    {
                        codeIsnb10 = code.Identifier;
                    }

                    if (code.Type == "ISBN_13")
                    {
                        codeIsnb13 = code.Identifier;
                    }
                } 
            }

            if (volumeInfo.Authors != null)
            {
                foreach (var aut in volumeInfo.Authors.ToList())
                {
                    if (auteur != "")
                    {
                        auteur += " ; ";
                    }
                
                    auteur += aut;
                }
            }

            //Configuration des paramètres
            var paramCodeIsbn10 = new SqlParameter("@CodeIsbn_10", SqlDbType.NVarChar)
            {
                Value = codeIsnb10
            };
            command.Parameters.Add(paramCodeIsbn10);

            var paramCodeIsbn13 = new SqlParameter("@CodeIsbn_13", SqlDbType.NVarChar)
            {
                Value = codeIsnb13
            };
            command.Parameters.Add(paramCodeIsbn13);

            var paramNom = new SqlParameter("@Nom", SqlDbType.NVarChar)
            {
                Value = volumeInfo.Title
            };
            command.Parameters.Add(paramNom);

            var paramSousTitre = new SqlParameter("@SousTitre", SqlDbType.NVarChar)
            {
                Value = volumeInfo.SubTitle??""
            };
            command.Parameters.Add(paramSousTitre);

            var paramImage = new SqlParameter("@Image", SqlDbType.NVarChar)
            {
                Value = volumeInfo.ImageLinks!=null?volumeInfo.ImageLinks.SmallThumbnail : ""
            };
            command.Parameters.Add(paramImage);

            var paramImage2 = new SqlParameter("@Image2", SqlDbType.NVarChar)
            {
                Value = volumeInfo.ImageLinks != null ? volumeInfo.ImageLinks.Thumbnail : ""
            };
            command.Parameters.Add(paramImage2);

            var paramAuteur = new SqlParameter("@Auteur", SqlDbType.NVarChar)
            {
                Value = auteur
            };
            command.Parameters.Add(paramAuteur);

            var paramAnneeEdition = new SqlParameter("@AnneeEdition", SqlDbType.Int)
            {
                Value = anneeEdition
            };
            command.Parameters.Add(paramAnneeEdition);

            command.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output; 
        }

        public void RemplirParametreNotesDeCours(SqlCommand command, NotesDeCoursModel notesModel)
        {
            var paramNom = new SqlParameter("@Nom", SqlDbType.NVarChar)
            {
                Value = notesModel.Nom
            };
            command.Parameters.Add(paramNom);

            var paramSousTitre = new SqlParameter("@SousTitre", SqlDbType.NVarChar)
            {
                Value = notesModel.SousTitre ?? ""
            };
            command.Parameters.Add(paramSousTitre);

            var paramMoisRedaction = new SqlParameter("@MoisRedaction", SqlDbType.Int)
            {
                Value = notesModel.MoisRedaction
            };
            command.Parameters.Add(paramMoisRedaction);

            var paramAnneeRedaction = new SqlParameter("@AnneeRedaction", SqlDbType.Int)
            {
                Value = notesModel.AnneeRedaction
            };
            command.Parameters.Add(paramAnneeRedaction);

            var paramMoisRevision = new SqlParameter("@MoisRevision", SqlDbType.Int)
            {
                Value = notesModel.MoisRevision
            };
            command.Parameters.Add(paramMoisRevision);

            var paramAnneeRevision = new SqlParameter("@AnneeRevision", SqlDbType.Int)
            {
                Value = notesModel.AnneeRevision
            };
            command.Parameters.Add(paramAnneeRevision);

            var paramCodeBarre = new SqlParameter("@CodeBarre", SqlDbType.NVarChar)
            {
                Value = notesModel.CodeBarre
            };
            command.Parameters.Add(paramCodeBarre);

            command.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
        }

       

        public void RemplirParametreOffre(SqlCommand command, OfferModel offre)
        {
            if (offre.TypeArticle == 1)
            {
                var paramIdLivre = new SqlParameter("@IdLivre", SqlDbType.Int)
                {
                    Value = offre.Livre.NoLivre
                };
                command.Parameters.Add(paramIdLivre);
            }
            else if(offre.TypeArticle == 2)
            {   
                var paramIdNotes = new SqlParameter("@IdNotes", SqlDbType.Int)
                {
                    Value = offre.NotesDeCours.NoNotesDeCours
                };
                command.Parameters.Add(paramIdNotes);
            }
            else if (offre.TypeArticle == 3)
            {
                var paramIdCalcu = new SqlParameter("@IdCalculatrice", SqlDbType.Int)
                {
                    Value = offre.Calculatrice.NoCalculatrice
                };
                command.Parameters.Add(paramIdCalcu);
            }

            var paramRemarques = new SqlParameter("@Remarques", SqlDbType.NVarChar)
            {
                Value = offre.Remarques ?? ""
            };
            command.Parameters.Add(paramRemarques);

            var paramEtat = new SqlParameter("@Etat", SqlDbType.NChar)
            {
                Value = offre.SelectedEtat
            };
            command.Parameters.Add(paramEtat);

            var paramDateFin = new SqlParameter("@CoursOblig", SqlDbType.NVarChar)
            {
                Value = offre.CoursObligatoires??""
            };
            command.Parameters.Add(paramDateFin);

            var paramStatut = new SqlParameter("@CoursRecom", SqlDbType.NVarChar)
            {
                Value = offre.CoursReferences??""
            };
            command.Parameters.Add(paramStatut);

            var paramUserId = new SqlParameter("@userId", SqlDbType.NVarChar)
            {
                Value = User.Identity.GetUserId()
            };
            command.Parameters.Add(paramUserId);

            var paramPrix = new SqlParameter("@Prix", SqlDbType.Float)
            {
                Value = offre.Prix
            };
            command.Parameters.Add(paramPrix);

            command.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output; 
        }

        private void publishToFacebook()
        {
            dynamic messagePost = new ExpandoObject();
            messagePost.access_token = "[YOUR_ACCESS_TOKEN]";
            messagePost.picture = "[A_PICTURE]";
            messagePost.link = "[SOME_LINK]";
            messagePost.name = "[SOME_NAME]";
            messagePost.caption = "{*actor*} " + "[YOUR_MESSAGE]"; //<---{*actor*} is the user (i.e.: Aaron)
            messagePost.description = "[SOME_DESCRIPTION]";

            var app = new FacebookClient("[YOUR_ACCESS_TOKEN]");

            try
            {
                var result = app.Post("/" + "http://www.facebook.com/likenson" + "/feed", messagePost);
            }
            catch (FacebookOAuthException ex)
            {
                 //handle something
            }
            catch (FacebookApiException ex)
            {
                 //handle something else
            }
        }

        public ActionResult DesactiverOffre(OfferModel offerModel)
        {
            var noOffre = Convert.ToInt32(Request.Form["NoOffre"]);

            offerModel.NoOffre = noOffre;

            var message="";
            TypeMessage type;

            if (UpdateOffre(offerModel))
            {
                message = "L'offre a été fermée.";
                type = TypeMessage.Succes;
            }
            else
            {
                message = "L'offre n'a pu être fermée.";
                type = TypeMessage.Erreur;
            }

            var offre = new OfferModel
            {
                Livre = new LivreModel {},
                ListeOffresUtil = ObtenirListeOffresUtil(),
                ListeOffresUtilNotesCours = ObtenirListeOffresUtilNotesCours(),
                ListeOffresUtilCalculatrices = ObtenirListeOffresUtilCalculatrice(),
                Message = message,
                TypeMessage = type
            };
            ViewBag.menuItemActive = "Offre";
            return View("ManageOffer", offre);
        }

       

        public bool UpdateOffre(OfferModel offre, int indActif = 0)
        {
            var update = false;

            var connetionString = Properties.Settings.Default.dbConnectionString;
            var sql = "Update Offre Set IndActif = '" + indActif + "', Raison = '" + offre.SelectedRaison + "', DetailsRaison = '" + offre.DétailsFermeture + "' Where Id = " + offre.NoOffre;
            var cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                command.ExecuteNonQuery();
                command.Dispose();
                cnn.Close();
                update = true;
            }
            catch (Exception ex)
            {
 
            }

            return update;
        }

        public ActionResult ManageOffer()
        {
            ViewBag.menuItemActive = "Offre";
            return View();
        }
     
    }   
}

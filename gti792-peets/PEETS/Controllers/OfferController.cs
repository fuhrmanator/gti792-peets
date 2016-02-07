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
            var offreModel = new OfferModel { Livre = new LivreModel { }, ListeOffresUtil = ObtenirListeOffresUtil() };
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
                         "JOIN Livre l On o.IdLivre = l.Id " +
                         "Where o.userId = '" + User.Identity.GetUserId() + "' And o.IndActif='1' order by o.Id desc";

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

        public ActionResult Create(OfferModel offre)
        {
                if (offre != null)
                {

                    int? noLivre = TraiterLivre(offre.Livre);
                    int? id = null;

                    if (noLivre != null)
                    {                      
                        offre.Livre.NoLivre = (int)noLivre;
  
                        var connetionString = Properties.Settings.Default.dbConnectionString;
                        string sql = "INSERT INTO Offre(IdLivre, Remarques, Etat, CoursOblig, CoursRecom, userId, Prix) OUTPUT Inserted.ID " +
                                           "VALUES(@IdLivre, @Remarques, @Etat, @CoursOblig, @CoursRecom, @userId, @Prix) SET @id=SCOPE_IDENTITY()";

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

                    offre.ListeOffresUtil = ObtenirListeOffresUtil();
                    offre.ListeOffresUtil.First(x => x.NoOffre == id).estNouv = true;
                    offre.Livre = new LivreModel {};
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
                         "l.CodeISBN_13, l.Nom, l.Image, o.Remarques, u.Email, u.PhoneNumber, l.Auteur, o.Prix " +
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
                        Prix = Convert.ToDouble(dataReader.GetValue(12).ToString())
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
            string sql = "INSERT INTO Livre(CodeIsbn_10, Nom, Image, Auteur, Image2, CodeIsbn_13, SousTitre) OUTPUT Inserted.ID " +
                         "VALUES(@CodeIsbn_10, @Nom, @Image, @Auteur, @Image2, @CodeIsbn_13, @SousTitre) SET @id=SCOPE_IDENTITY()";

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
        public void RemplirParametreLivre(SqlCommand command, VolumeInfo volumeInfo)
        {
            var codeIsnb10 = "";
            var codeIsnb13 = "";
            var auteur = "";

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

            command.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output; 
        }

        public void RemplirParametreOffre(SqlCommand command, OfferModel offre)
        {
            var paramIdLivre = new SqlParameter("@IdLivre", SqlDbType.NVarChar)
            {
                Value = offre.Livre.NoLivre
            };
            command.Parameters.Add(paramIdLivre);

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

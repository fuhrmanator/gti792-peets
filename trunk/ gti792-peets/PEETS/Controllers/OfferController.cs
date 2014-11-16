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
                         "Where o.userId = '" + User.Identity.GetUserId() + "' And o.IndActif='1'";

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

        public ActionResult Create(OfferModel offre)
        {
                if (offre != null)
                {

                    int? noLivre = TraiterLivre(offre.Livre);

                    if (noLivre != null)
                    {
                        offre.Livre.NoLivre = (int)noLivre;
  
                        var connetionString = Properties.Settings.Default.dbConnectionString;
                        const string sql = "INSERT INTO Offre(IdLivre, Remarques, Etat, CoursOblig, CoursRecom, userId) VALUES(@IdLivre, @Remarques, @Etat, @CoursOblig, @CoursRecom, @userId)";          
                        var cnn = new SqlConnection(connetionString);

                        try
                        {
                            cnn.Open();

                            var command = new SqlCommand(sql, cnn);

                            RemplirParametreOffre(command, offre);

                            command.ExecuteNonQuery();
                            command.Dispose();
                            cnn.Close();

                            //publishToFacebook();

                            offre.Message = "L'offre a été créée avec succès.";
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
                    offre.Livre = new LivreModel {};
                }
               
                return View("ManageOffer", offre);
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
                        auteur += " , ";
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
                message = "L'offre a été fermé.";
                type = TypeMessage.Succes;
            }
            else
            {
                message = "L'offre n'a pu être fermé.";
                type = TypeMessage.Erreur;
            }

            var offre = new OfferModel
            {
                Livre = new LivreModel {},
                ListeOffresUtil = ObtenirListeOffresUtil(),
                Message = message,
                TypeMessage = type
            };
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
            return View();
        }
     
    }   
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.Versioning;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Windows.Forms;
using ComputerBeacon.Facebook.Graph;
using Microsoft.Ajax.Utilities;
using PEETS.Models;
using System.Data.SqlClient;

namespace PEETS.Controllers
{
    public class LivreController : Controller
    {
        // Create: Livre
        [WebMethod]
        public ActionResult Create(LivreBean livreModel)
        {

            if (livreModel != null)
            {                
                    SqlConnection cnn = null;
                    string connetionString = Properties.Settings.Default.dbConnectionString;
                    string sql = "INSERT INTO Livre(CodeISBN, Nom, Image, EtatLivre) VALUES(@CodeIsbn, @Nom, @Image, @EtatLivre)";
                    var fileBytes = new byte[0];

                    HttpPostedFileBase file = Request.Files["Livre.Image"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    }                   

                    cnn = new SqlConnection(connetionString);

                    try
                    {
                        cnn.Open();

                        var command = new SqlCommand(sql, cnn);

                        var paramCodeIsbn = new SqlParameter("@CodeIsbn", SqlDbType.NVarChar)
                        {
                            Value = livreModel.CodeIsbn ?? ""
                        };
                        command.Parameters.Add(paramCodeIsbn);

                        var paramNom = new SqlParameter("@Nom", SqlDbType.NVarChar)
                        {
                            Value = livreModel.NomLivre
                        };
                        command.Parameters.Add(paramNom);

                        var paramFileField = new SqlParameter("@Image", SqlDbType.Image)
                        {
                            Value = fileBytes
                        };
                        command.Parameters.Add(paramFileField);

                        var paramEtat = new SqlParameter("@EtatLivre", SqlDbType.NChar)
                        {
                            Value = livreModel.NoLivre
                        };
                        command.Parameters.Add(paramEtat);
          
                        command.ExecuteNonQuery();
                        command.Dispose();
                        cnn.Close();
                    }
                    catch (Exception ex)
                    {
                        
                    }
               
            }

            ViewBag.menuItemActive = "Livre";
            return View("/Views/Account/Manage.cshtml");
        }

        public ActionResult ObtenirListeLivres()
        {
           // List<LivreModel> 
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            string sql = "SELECT * FROM Livre";
         
            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                command = new SqlCommand(sql, cnn);

                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                   // MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                }

                dataReader.Close();      
                command.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Can not open connection ! ");
            }

            return View("/Views/Home/Index.cshtml");
        }

        public JsonResult ObtenirNomLivres(int isbn)
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var nomLivres = new List<NomAutoComplete>();
            var sql = string.Format("SELECT distinct CodeISBN_13, Nom FROM Livre " + "Where codeisbn_10 like '%{0}%' or codeisbn_13 like '%{0}%'", isbn);

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                var dataReader = command.ExecuteReader();
                
                while (dataReader.Read())
                {
                    var nomAuto = new NomAutoComplete
                    {
                        CodeIsbn = dataReader.GetValue(0).ToString(),
                        Nom = dataReader.GetValue(1).ToString()
                    };

                    nomLivres.Add(nomAuto);
                }

                dataReader.Close();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception e)
            {

            }

            return Json(nomLivres, JsonRequestBehavior.AllowGet);
        }
    }

}
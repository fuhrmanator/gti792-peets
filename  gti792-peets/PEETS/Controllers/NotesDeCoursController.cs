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
    public class NotesDeCoursController : Controller
    {
        // Create: Notes de cours
        [WebMethod]
        public ActionResult Create(NotesDeCoursBean notesModel)
        {

            if (notesModel != null)
            {
                SqlConnection cnn = null;
                string connetionString = Properties.Settings.Default.dbConnectionString;
                string sql = "INSERT INTO NotesDeCours(Nom,SousTitre,MoisRedaction ,AnneeRedaction, MoisRevision,AnneeRevision, CodeBarre) VALUES(@Nom,@SousTitre, @MoisRedaction, @AnneeRedaction, @MoisRevision,@AnneeRevision,@CodeBarre)";
                var fileBytes = new byte[0];

                /*HttpPostedFileBase file = Request.Files["Livre.Image"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }*/

                cnn = new SqlConnection(connetionString);

                try
                {
                    cnn.Open();

                    var command = new SqlCommand(sql, cnn);

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
                   

                    command.ExecuteNonQuery();
                    command.Dispose();
                    cnn.Close();
                }
                catch (Exception ex)
                {

                }

            }

            ViewBag.menuItemActive = "Notes de cours";
            return View("/Views/Account/Manage.cshtml");
        }

        public ActionResult ObtenirListeNotesDeCours()
        {
            // List<LivreModel> 
            SqlConnection cnn = null;
            string connetionString = Properties.Settings.Default.dbConnectionString;
            SqlCommand command = null;
            string sql = "SELECT * FROM NotesDeCours";

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

        public JsonResult ObtenirNomNotesDeCours(int codeBarre)
        {
            SqlConnection cnn = null;
            var connetionString = Properties.Settings.Default.dbConnectionString;
            var nomNotesDeCours = new List<NomNotesCoursAutoComplete>();
            var sql = string.Format("SELECT distinct CodeBarre, Nom FROM NotesDeCours " + "Where CodeBarre like '%{0}%' ", codeBarre);

            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                var command = new SqlCommand(sql, cnn);
                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    var nomAuto = new NomNotesCoursAutoComplete
                    {
                        CodeBarre = dataReader.GetValue(0).ToString(),
                        Nom = dataReader.GetValue(1).ToString()
                    };

                    nomNotesDeCours.Add(nomAuto);
                }

                dataReader.Close();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception e)
            {

            }

            return Json(nomNotesDeCours, JsonRequestBehavior.AllowGet);
        }
    }

    
}
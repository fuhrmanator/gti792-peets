﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace PEETS.Models
{
    public class OffreBean
    {
        public string CodeIsbn_13;
        public int NoOffre { get; set; }

        public string NomLivre { get; set; }
        public string NomNotesCours { get; set; }
        public Boolean estNouv { get; set; }
        public string CodeIsbn_10 { get; set; }

        public string ImageLivre { get; set; }

        public string EtatLivre { get; set; }

        public string Remarques { get; set; }
        public string CoursRecommandes { get; set; }
        public string CoursObligatoires { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string Auteur { get; set; }
        public string SousTitre { get; set; }
        public bool Statut { get; set; }
        public double Prix { get; set; }
        public string AnneeEdition { get; set; }
        public string MoisRedaction { get; set; }
        public int AnneeRedaction { get; set; }
        public string MoisRevision { get; set; }
        public int AnneeRevision { get; set; }
        public string ModeleCalculatrice { get; set; }
        public string TypeArticle { get; set; }
        public string CodeBarre { get; set; }
        public static int GetTotalRows(string recherch)
        {
            var totalRows = 0;

            string connetionString = Properties.Settings.Default.dbConnectionString;
            var cnn = new SqlConnection(connetionString);
            cnn.Open();
            string sql = "SELECT Count(o.Id) " +
                               "FROM dbo.Offre o " +
                               "JOIN Livre l On o.IdArticle = l.Id " +
                               "JOIN dbo.Etat e ON o.Etat = e.CodeEtat " +
                               "Where o.IndActif = '1' AND o.IdTypeArticle = 1 " + recherch;

            var command = new SqlCommand(sql, cnn);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                totalRows = (int)dataReader.GetValue(0);
            }

            return totalRows;
        }

        public static int GetTotalRowsNotes(string recherch)
        {
            var totalRows = 0;

            string connetionString = Properties.Settings.Default.dbConnectionString;
            var cnn = new SqlConnection(connetionString);
            cnn.Open();
            string sql = "SELECT Count(o.Id) " +
                               "FROM dbo.Offre o " +
                               "JOIN NotesDeCours n On o.IdArticle = n.IdNotesDeCours " +
                               "JOIN dbo.Etat e ON o.Etat = e.CodeEtat " +
                               "Where o.IndActif = '1' AND o.IdTypeArticle = 2 " + recherch;

            var command = new SqlCommand(sql, cnn);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                totalRows = (int)dataReader.GetValue(0);
            }

            return totalRows;
        }

        public static int GetTotalRowsCalcu(string recherch)
        {
            var totalRows = 0;

            string connetionString = Properties.Settings.Default.dbConnectionString;
            var cnn = new SqlConnection(connetionString);
            cnn.Open();
            string sql = "SELECT Count(o.Id) " +
                               "FROM dbo.Offre o " +
                               "JOIN Calculatrice c On o.IdArticle = c.IdCalculatrice " +
                               "JOIN dbo.Etat e ON o.Etat = e.CodeEtat " +
                               "Where o.IndActif = '1' AND o.IdTypeArticle = 3 " + recherch;

            var command = new SqlCommand(sql, cnn);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                totalRows = (int)dataReader.GetValue(0);
            }

            return totalRows;
        }

        public IEnumerable<SelectListItem> TriItems = new List<SelectListItem>
        {
            new SelectListItem  {Value = "l.Nom", Text = "Nom"}, 
            new SelectListItem  {Value = "o.Etat", Text = "État"}, 
            new SelectListItem  {Value = "l.CodeISBN_13", Text = "Code Isbn"}, 
            new SelectListItem {Value = "l.Auteur", Text = "Auteur"}
        };

        public string SelectedTriItem { get; set; }

        public IEnumerable<SelectListItem> OrdreItems = new List<SelectListItem>
        {
            new SelectListItem  {Value = "ASC", Text = "Croissant"}, 
            new SelectListItem  {Value = "DESC", Text = "Décroissant"}
        };

        public string SelectedOrdreItem { get; set; }


    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PEETS
{
    using System;
    using System.Collections.Generic;
    
    public partial class Offre
    {
        public Offre()
        {
            this.AspNetUsers = new HashSet<AspNetUser>();
        }
    
        public int Id { get; set; }
        public int IdLivre { get; set; }
        public string Remarques { get; set; }
        public string Etat { get; set; }
        public string CoursOblig { get; set; }
        public string CoursRecom { get; set; }
        public string userId { get; set; }
        public string IndActif { get; set; }
        public string Raison { get; set; }
        public string DetailsRaison { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Etat Etat1 { get; set; }
        public virtual Livre Livre { get; set; }
        public virtual Raison Raison1 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}

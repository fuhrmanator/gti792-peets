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
    
    public partial class Livre_Cours_TypeReferences
    {
        public int IdLivre { get; set; }
        public string SigleCours { get; set; }
        public string TypeReferences { get; set; }
    
        public virtual Cours Cour { get; set; }
        public virtual Livre Livre { get; set; }
    }
}
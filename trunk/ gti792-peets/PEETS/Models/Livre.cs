using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEETS.Models
{
    public class Livre
    {
        public int NoLivre { get; set; }
        public String NomLivre { get; set; }

        public string CodeIsbn { get; set; }
    }
}
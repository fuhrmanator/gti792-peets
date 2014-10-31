function DetailsOnSuccess(data) {

    if (data != null) {

        var data2 = JSON.stringify(data);
        var detail = JSON.parse(data2);

        $("#headerDetails").text(detail.NomLivre);
        $("#nomLivre").text(detail.NomLivre);
        $("#sousTitre").text(detail.SousTitre);
        $("#codeIsbn10").text(detail.CodeIsbn_10);
        $("#codeIsbn13").text(detail.CodeIsbn_13);
        $("#coursOblig").text(detail.CoursObligatoires);
        $("#coursRecom").text(detail.CoursRecommandes);
        $("#auteurLivre").text(detail.Auteur);
        $("#remLivre").text(detail.Remarques);
        $("#etatLivre").text(detail.EtatLivre);
        $("#courrProprio").text(detail.Email);
        $("#telProprio").text(detail.Phone);

        $("#detailsDialog").modal();
    }
   
}

function DetailsOnFailure(data) {
    alert('La requête a échouée.');
}

function Rechercher() {

   var nom = $("#nom").val();
   var isbn = $("#isbn").val();
   var auteur = $("#auteur").val();
   var sigle = $("#sigle").val();
   var pageActuel = $("#pageActuel").val();

   var recherche = { titre: nom, isbn: isbn, auteur: auteur, sigle: sigle, pageActuel: pageActuel };
   $.ajax({
       type: "POST",
       url: "/Home/Rechercher",
       data: recherche,
       datatype: "html",
       success: function (data) {          
           var result = $(data).find("#affichageLivre");
           $("#affichageLivre").html(result);
       }
   });
}
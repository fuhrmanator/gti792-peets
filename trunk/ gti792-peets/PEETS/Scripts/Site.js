$(function () {
   
    $("#Livre_CodeIsbn").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Livre/ObtenirNomLivres',
                type: "POST",
                dataType: "json",
                data: { isbn: request.term },
                success: function (data) {
                    //alert(data);
                    response($.map(data, function(item) {
                        return { label: item.Nom + "(ISBN 13: " + item.CodeIsbn + ")", value: item.CodeIsbn };
                    }));
                }
            });
        },
        messages: {
            noResults: "", results: ""
        },
        minLength: 2,
        select: function (event, ui) {
            $('#Livre_CodeIsbn').val(ui.item.code);
        }
    });

});

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

function GererPagination(command) {

    var nom = $("#nom").val();
    var isbn = $("#isbn").val();
    var auteur = $("#auteur").val();
    var sigle = $("#sigle").val();
    var pageActuel = $("#pageActuel").val();
    //@Html.ActionLink("Suivant >>", "ObtenirListeLivresParPage", new { page = "+>1", pageActuel = pageActuel, pageTotal = pageCount })
    //@Html.ActionLink("<< Précédent", "ObtenirListeLivresParPage", new { page = "<-1", pageActuel = pageActuel, pageTotal = pageCount })

    var recherche = { titre: nom, isbn: isbn, auteur: auteur, sigle: sigle, pageActuel: pageActuel };
    $.ajax({
        type: "POST",
        url: "/Home/ObtenirListeLivresParPage",
        data: recherche,
        datatype: "html",
        success: function (data) {
            var result = $(data).find("#affichageLivre");
            $("#affichageLivre").html(result);
        }
    });
}

function AfficherModalFermeture(id, nom) {
    $("#lbNomFerme").text(nom);
    $("#NoOffre").val(id);
   $("#fermerOfferDialog").modal();
}

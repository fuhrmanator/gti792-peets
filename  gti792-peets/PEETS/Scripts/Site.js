$(function () {
   
    $("#Livre_CodeIsbn").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Livre/ObtenirNomLivres',
                type: "POST",
                dataType: "json",
                data: { isbn: request.term },
                success: function (data) {
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

    $("#Accueil, #Compte, #Offre, #About, #Contact").click(function () {

        $("#Accueil").removeClass("active");
        $("#Compte").removeClass("active");
        $("#Offre").removeClass("active");
        $("#About").removeClass("active");
        $("#Contact").removeClass("active");

        $("#" + this.id + "").addClass("active");

    });

    if ($("#nomPage").val() == "Acceuil") {
        $("#recherche").show();
    } else {
        $("#recherche").hide();
    }

    $("#Livre_CodeIsbn").focusout(function () {

        var codeIsbn = $("#Livre_CodeIsbn").val();

        $("#titreLivre").text("");
        $("#ateurLivre").text("");

        if (codeIsbn.length >= 10) {

            var codeIsbn2 = { codeIsbn: codeIsbn };
            
            $.ajax({
                type: "POST",
                url: "/Offer/GetInfoLivreJson",
                data: codeIsbn2,
                datatype: "html",
                success: function (data) {
                    var data2 = JSON.stringify(data);
                    var detail = JSON.parse(data2);
                    $("#titreLivre").text(detail.NomLivre);
                    $("#ateurLivre").text(detail.Auteur);
                },
                error: function (resultat, statut, erreur) {
                    alert(erreur);
                }
            });
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
        $("#courrProprio").attr("href", "mailto:" + detail.Email);
        $("#courrProprio").text(detail.Email);
        $("#telProprio").text(detail.Phone);

        $("#detailsDialog").modal();
    }
   
}

function ModifierOffre() {


    var idOffre = $("#idOffre").text();
    var coursObligModif = $("#coursObligModif").val();
    var coursRecomModif = $("#coursRecomModif").val();
    var selectedEtatVal = $('#SelectedEtatModif').val();
    var remLivreModif = $("#remLivreModif").val();

    var modifValues = { noOffre: idOffre, coursOblig: coursObligModif, coursRecom: coursRecomModif, etat: selectedEtatVal, rem: remLivreModif };

    $.ajax({
        type: "POST",
        url: "/Offer/Modifier",
        data: modifValues,
        datatype: "html",
        success: function (data) {
            alert("L'offre " + idOffre + " a été modifiée avec succès.");
            $("#modifDialog").modal('hide');
        },
        error: function (resultat, statut, erreur) {
            alert(erreur);
        }
    });
}

function ModifOnSuccess(data) {

    if (data != null) {

        var data2 = JSON.stringify(data);
        var detail = JSON.parse(data2);
        $("#idOffre").text(detail.NoOffre);
        $("#nomLivreModif").text(detail.NomLivre);
        $("#coursObligModif").val(detail.CoursObligatoires);
        $("#coursRecomModif").val(detail.CoursRecommandes);
        $('#SelectedEtatModif option:selected').text(detail.EtatLivre);
        $("#remLivreModif").val(detail.Remarques);
        
        $("#modifDialog").modal();
    }

}

function DetailsOnFailure(data) {
    alert('La requête a échouée.');
}

function afficherGifAttente() {
    var opts = {
        lines: 13, // The number of lines to draw
        length: 40, // The length of each line
        width: 11, // The line thickness
        radius: 38, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 0, // The rotation offset
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#000', // #rgb or #rrggbb or array of colors
        speed: 2.2, // Rounds per second
        trail: 67, // Afterglow percentage
        shadow: true, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: '50%', // Top position relative to parent
        left: '50%' // Left position relative to parent
    };
    var target = document.getElementById('labelNbLivre');
    var spinner = new Spinner(opts).spin(target);
}

function Rechercher() {

    afficherGifAttente();

   var nom = $("#nom").val();
   var isbn = $("#isbn").val();
   var auteur = $("#auteur").val();
   var sigle = $("#sigle").val();
   var pageActuel = $("#pageActuel").val();
   var triItem = $("#SelectedTriItem").val();
   var ordreItem = $("#SelectedOrdreItem").val();

   var recherche = { titre: nom, isbn: isbn, auteur: auteur, sigle: sigle, pageActuel: pageActuel, tri: triItem, ordre: ordreItem };
   
   $.ajax({
       type: "POST",
       url: "/Home/Rechercher",
       data: recherche,
       datatype: "html",
       success: function (data) {

           GererResultat(data);

       },
       error: function (resultat, statut, erreur) {
           alert(erreur);
       } 
   });
}

function Trier() {

    afficherGifAttente();

    var nom = $("#nom").val();
    var isbn = $("#isbn").val();
    var auteur = $("#auteur").val();
    var sigle = $("#sigle").val();
    var triItem = $("#SelectedTriItem").val();
    var ordreItem = $("#SelectedOrdreItem").val();
    var pageActuel = $("#pageActuel").val();
   
    var tri = { tri: triItem, ordre: ordreItem, pageActuel: pageActuel, titre: nom, isbn: isbn, auteur: auteur, sigle: sigle };

    $.ajax({
        type: "POST",
        url: "/Home/Trier",
        data: tri,
        datatype: "html",
        success: function (data) {
            GererResultat(data);
        }
    });
}

function GererResultat(data) {  
    var result = $(data).find("#listeOffre");
    $("#listeOffre").html(result);  
}

function GererPagination(command) {

    afficherGifAttente();

    var nom = $("#nom").val();
    var isbn = $("#isbn").val();
    var auteur = $("#auteur").val();
    var sigle = $("#sigle").val();
    var pageActuel = $("#pageActuel").val();
    var triItem = $("#SelectedTriItem").val();
    var ordreItem = $("#SelectedOrdreItem").val();

    var recherche = { pageDemande: command, pageActuel: pageActuel, titre: nom, isbn: isbn, auteur: auteur, sigle: sigle, tri: triItem, ordre: ordreItem };
    $.ajax({
        type: "POST",
        url: "/Home/ObtenirListeLivresParPage",
        data: recherche,
        datatype: "html",
        success: function (data) {
            GererResultat(data);
        }
    });
}

function AfficherModalFermeture(id, nom) {
    $("#lbNomFerme").text(nom);
    $("#NoOffre").val(id);
   $("#fermerOfferDialog").modal();
}

// Shorthand for $( document ).ready()
$(function () {


});

function addOffer() {

    var $addOfferDialog = $("#addOfferDialog");
    var code = $("#code").val();
    var nom = $("#nom").val();
    var etat = $("#etat").val();
    var image = $("#image").val();
    var prix = $("#prix").val();

    //var jsonLivre = [
    //    { code: code, Nom: nom, Etat: etat, Image: image, Prix: prix }
    //];

     $.ajax({
         url: '/Livre/Create',
        type: 'POST',
        data: {code: code},
        success: function (result) {
            $addOfferDialog.modal('hide');
        },
        error: function (result) {
         alert("Une erreur s'est produite");
     }
    });
}
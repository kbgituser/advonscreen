// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {


    //$(".allownumericwithdecimal").on("keypress keyup blur", function (event) {
    //    //this.value = this.value.replace(/[^0-9\.]/g,'');
    //    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
    //    if ((event.which != 46 || $(this).val().indexOf('.') != -1)
    //        && (event.which < 48 || event.which > 57)
    //        && (event.which != 13)
    //    ) {
    //        event.preventDefault();
    //    }
    //});


    $(".allownumericwithdecimal").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57 || event.whitch === 188 || event.which === 110)) {
            event.preventDefault();
        }
    });

    $(".allownumericwithoutdecimal").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if ((event.which < 48 || event.which > 57) && (event.which != 13)) {
            event.preventDefault();
        }
    });

});
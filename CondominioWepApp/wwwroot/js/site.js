// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#myInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".searching").filter(function () {
            // Obtén el valor del atributo data-search de la tarjeta
            var searchValue = $(this).data("search").toLowerCase();
            $(this).toggle(searchValue.indexOf(value) > -1);
        });
    });
});

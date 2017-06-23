var Travel = require('../js/travel.js').travelObject;
$(document).ready(function () {
    var newTravel = new Travel();
    $("#customer").submit(function (event) {
        event.preventDefault();
        $("#hide").removeClass("hidden");
        $("#weather, #budgetConvert, #restaurant, #hotel").text("");
        newTravel.place = $("#destination").val().replace(" ", "_").toLowerCase();
        newTravel.getWiki();
        var newBudget = parseFloat($("#budget").val());
        var newPosition = newTravel.getInfo(newBudget);
        newTravel.getRestaurants();
        newTravel.getHotels();
        newTravel.getWeather();
    });

});


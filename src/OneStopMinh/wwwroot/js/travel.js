function Travel() {
    this.place = "";
}

Travel.prototype.getWiki = function () {
    $.ajax({
        url: "Home/Wiki",
        type: 'POST',
        dataType: "json",
        data: { place: this.place },
        success: function (result) {
            var markup = result.parse.text["*"];
            var i = $('<div></div>').html(markup);
            i.find('a').each(function () { $(this).replaceWith($(this).html()); });
            i.find('sup').remove();
            i.find('.mw-ext-cite-error').remove();
            $('#info').html($(i).find('p').has('b'));
            $('#info-picture').html($(i).find('img').first());
        }
    });
};

Travel.prototype.getInfo = function (budget) {
    var position = [];
    $.ajax({
        url: "Home/Coord",
        type: 'POST',
        dataType: "json",
        data: { place: this.place },
        success: function (response) {
            console.log(response);
            position.push(response.Response.View[0].Result[0].Location.DisplayPosition.Latitude);
            position.push(response.Response.View[0].Result[0].Location.DisplayPosition.Longitude);
            position.push(response.Response.View[0].Result[0].Location.Address.Country.toLowerCase());
            position.push(response.Response.View[0].Result[0].Location.Address.PostalCode);
        }
    }).then(function () {
        getCurrency(position[2], budget);
        getAttractions(position[0], position[1]);
        getAirport(position[0], position[1]);
    });
    return position;
};

Travel.prototype.getRestaurants = function () {
    $.ajax({
        url: "Home/Restaurants",
        type: 'POST',
        dataType: "json",
        data: { place: this.place },
        success: function (response) {
            var restaurants = response['businesses'];
            restaurants.forEach(function (item) {
                $("#restaurant").append("<div class='col-md-1 newRes'>" + "<img class='pics' src=" + item.image_url + ">" + "<br>" + "<a href=" + item.url + ">" + item.name + "</a>" + "<br>" + "<p>" + item.rating + "&#9733" + "</p>" + "</div>");
            });
        }
    });
};

Travel.prototype.getHotels = function () {
    $.ajax({
        url: "Home/Hotels",
        type: 'POST',
        dataType: "json",
        data: { place: this.place },
        success: function (response) {
            var hotels = response['businesses'];
            hotels.forEach(function (item) {
                $("#hotel").append("<div class='col-md-1 newHotel'>" + "<img class='pics' src=" + item.image_url + ">" + "<br>" + "<a href=" + item.url + ">" + item.name + "</a>" + "<br>" + "<p>" + item.rating + "&#9733" + "</p>" + "</div>");
            });
        }
    });
};


Travel.prototype.getWeather = function () {
    $.ajax({
        url: "Home/Weather",
        type: 'POST',
        dataType: "json",
        data: { place: this.place },
        success: function (response) {
            var forecast = response.list;
            var chosen = [6, 14, 22, 30, 38];
            for (var j = 0; j < chosen.length; j++) {
                var getDate = forecast[chosen[j]].dt_txt.toString().slice(0, 10);
                var toFarenheit = (parseFloat(forecast[chosen[j]].main.temp_min) * 9 / 5 - 460).toFixed(2);
                $("#weather").append("<div class='col-md-2'>" + "<div class='panel panel-default'>" + "<div class='panel-heading'>" + "Date: " + getDate + "</div>" + "<div class='panel-body'>" + " Temperature: " + toFarenheit + "<br>" + "Forecast: " + forecast[chosen[j]].weather[0].main + "</div>" + "</div>" + "</div>");
            }
        }
    });
};
var getCurrency = function (countryCode, budget) {
    $.ajax({
        url: "Home/CurrencyCode",
        type: 'POST',
        dataType: "json",
        data: { countryCode: countryCode },
        success: function (response) {
            var currency = response.currencies[0].code;
            $('#currency').text(currency);
            if (currency !== "USD") {
                getExchange(currency, budget);
            }
        }
    });
};

var getExchange = function (foreign, budget) {
    $.ajax({
        url: "Home/Exchange",
        type: 'POST',
        dataType: "json",
        data: { currencyCode: foreign },
        success: function (response) {
            var temp = response.quotes;
            var rate = temp[Object.keys(temp)[1]];
            $("#rate").text(rate.toString());
            $("#convert").text(parseFloat(rate * budget).toFixed(2));
        }
    });
};

var getAttractions = function (lat, long) {
    $.ajax({
        url: "Home/Attractions",
        type: 'POST',
        dataType: "json",
        data: { latitude: lat, longitude: long },
        success: function (response) {
            var attractions = response.results.items;
            attractions.forEach(function (item) {
                var itemValue = item.title.toString() + "";
                console.log(itemValue);
                $('#attractions ul').append('<li>' + '<form id="' + 'saveAttractions' + attractions.indexOf(item) + '">' + '<input type="hidden" name="attractionName" value=' + itemValue + '/>' + item.title + '<button type="submit" class="btn btn-success">Save</button>' + '</form>' + '</li>')
            });
            $("[id^=saveAttractions]").on("submit", function (event) {
                event.preventDefault();
                $.ajax({
                    url: "Home/SaveAttractions",
                    type: 'POST',
                    dataType: "json",
                    data: $(this).serialize(),
                    success: function (response) {
                        console.log(response);
                    }
                });
            });
        }
    });
}
var getAirport = function (lat, long) {
    //$.ajax({
    //    url: "Home/Airport",
    //    type: 'POST',
    //    dataType: "json",
    //    data: { latitude: lat, longitude: long },
    //    success: function (response) {
    //        console.log(response);
    //    }
    //}).then(function () {



    //});
    var request =
  {
      "request": {
          "passengers": {
              "adultCount": 1
          },
          "slice": [
           {
               "origin": "SEA",
               "destination": "LAX",
               "date": "2017-6-30"
           }
          ]
      }
  }


    $.ajax({
        url: "Home/Flights",
        type: 'POST',
        dataType: 'json',
        data: { queries: "{\"request\":{\"passengers\":{\"adultCount\":1}\"slice\":[{\"origin\":\"SEA\"\"destination\":\"LAX\"\"date\":\"2017-6-30\"}]}}" },
        success: function (response) {
            console.log(response);
        }
    });
}
exports.travelObject = Travel;

// Passenger View Javascripts

/*
Function: getDepartureRouteList()
Get all routes from the Bus Tracker API and append the results to the
departureSelect drop down.
*/
function getDepartureRouteList() {
    $.getJSON(apiBaseUrl + apiGetRoutesUrl, function (data) {
        $.each(data, function (index) {
            $('#routeSelect')
                .append($('<option>', { value: data[index].RouteId })
                    .text(data[index].RouteName + ' (' + data[index].RouteNumber + ')')); 
        });
    });
}

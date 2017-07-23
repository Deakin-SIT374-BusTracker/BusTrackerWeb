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

/*
Event Handler: routeSelect.change()
On route selection get all route directions from the Bus Tracker API and append
the results to the directionSelect drop down.
*/
$("#routeSelect").change(function () {
    var selectedRouteId = $('#routeSelect option:selected').val();
    var jsonUrl = apiBaseUrl + apiGetRouteDirectionsUrl + 'routeId=' + selectedRouteId;

    $.getJSON(jsonUrl, function (data) {
        $.each(data, function (index) {
            $('#directionSelect')
                .append($('<option>', { value: data[index].DirectionId })
                    .text(data[index].DirectionName));
        });
    });
});


 // Passenger View Javascripts

/*
Function: getDepartureRouteList()
Get all routes from the Bus Tracker API and append the results to the
departureSelect drop down.
*/
$('#departureSelect').change(function () {
    $.getJSON(apiBaseUrl + apiGetRoutesUrl, function (data) {
        $.each(data, function (index) {
            $('#routeSelect')
                .append($('<option>', { value: data[index].RouteId })
                    .text(data[index].RouteName + ' (' + data[index].RouteNumber + ')')); 
        });
    });
}

/*
Function: buildScheduleTable()
Dynamically build the schedule table with the route next run data.
*/
function buildScheduleTable(data) {
    $.each(data.StoppingPattern.Departures, function (index, departure) {
        var stopName = departure.Stop.StopName;
        var scheduledDeparture = departure.ScheduledDeparture;
        var estimatedDeparture = departure.EstimatedDeparture;

        $('#scheduleTable')
            .append($('<tr><td><span style="font-weight:bold">' + stopName + '</span><br />Scheduled Time: ' + scheduledDeparture + '<br />Estimated Time: ' + estimatedDeparture + '</td></tr>'));
    });
}

/*
Event Handler: routeSelect.change()
On route selection get all route directions from the Bus Tracker API and append
the results to the directionSelect drop down.
*/
$('#routeSelect').change(function () {
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

/*
Event Handler: searchButton.click()
On search button click get the route next run data from the Bus Tracker API,
then build the schedule table and google map.
*/
$('#searchButton').click(function () {
    var selectedRouteId = $('#routeSelect option:selected').val();
    var selecteddirectionId = $('#directionSelect option:selected').val();
    var jsonUrl = apiBaseUrl + apiGetNextRouteRunUrl + 'routeId=' + selectedRouteId + '&directionId=' + selecteddirectionId;
        
    $.getJSON(jsonUrl, function (data) {
        buildScheduleTable(data);
    });
});



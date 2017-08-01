$(function () {
    $('#searchResults').hide();
});
/*
Function: buildRouteTable()
Dynamically build the route table with the route next run data.
*/
$('#searchButton').click(function () {
    //$.each(data.StoppingPattern.Departures, function (index, departure) {
    //    var stopName = departure.Stop.StopName;
    //    var scheduledDeparture = departure.ScheduledDeparture;
    //    var estimatedDeparture = departure.EstimatedDeparture;

    $('#searchResults').show();

    $('#routeTable')
        .append($('<tr><td>Deakin via Grovedale</td><td>Deakin University</td><td><button type="button" class="btn btn-default">Go</button></td></tr>'));
    $('#routeTable')
        .append($('<tr><td></td><td>Geelong Station</td><td><button type="button" class="btn btn-default">Go</button></td></tr>'));

});
$(document).ready(function(){
	var routeModal_title,
		routeModal_body,
		routeInfoModal_title,
		routeInfoModal_body,
		$routes_group = {};
	
	var domain = "http://bustrackerweb.azurewebsites.net";
	
	routeModal_title = $("#routeModal .panel-title");
	routeModal_body = $("#routeModal .modal-body");
	routeInfoModal_title = $("#routeInfoModal .panel-title");
	routeInfoModal_body = $("#routeInfoModal .modal-body");
	
	jQuery.ajax({
		type: "GET",
		url: domain + "/api/Route/GetRoutes",
		headers: {          
			Accept: "application/xml"  
		},
		dataType: "text",
		success: function (data) {
			var xmlDoc = $.parseXML(data),
				$xml = $(xmlDoc);
			var $routes = $xml.find('RouteModel');
			
			$(".routes-badge").text($routes.length);
						
			var stops = {};
			
			$routes.each(function () {
				var routename = $(this).find('RouteName').text();
				var index_seperator = routename.indexOf('-');
				var index_to = routename.indexOf('to');
				if(routename[index_seperator + 1] == ' ') {
					routename = routename.substring(0, index_seperator != -1 ? index_seperator : routename.length);
					routename = routename.trim();
				} else if (routename[index_to + 2] == ' ') {
					routename = routename.substring(0, index_to != -1 ? index_to : routename.length);
					routename = routename.trim();
				}
				if(!stops[routename]) {
					stops[routename] = true;
				}
			});
			
			for(var stop in stops) {
				var $route_stops = [];
				
				$routes.each(function () {
					var routename = $(this).find('RouteName').text();
					var index_seperator = routename.indexOf('-');
					var index_to = routename.indexOf('to');
					
					if(routename[index_seperator + 1] == ' ') {
						routename = routename.substring(0, index_seperator != -1 ? index_seperator : routename.length);
						routename = routename.trim();
					} else if (routename[index_to + 2] == ' ') {
						routename = routename.substring(0, index_to != -1 ? index_to : routename.length);
						routename = routename.trim();
					}
										
					if(stop == routename) {
						$route_stops.push($(this));
					}
				});
				$routes_group[stop] = $route_stops;
			};
			
			$.each($routes_group, function(route_name, routes) {
				$(".routes-group-list").append(
					"<a href=\"javascript:void(0)\" class=\"list-group-item justify-content-between\"><span class=\"badge badge-default badge-pill\">"
					+ routes.length
					+ "</span>"
					+ route_name
					+ "</a>"
				);
			});
		}
	});
	
	$('#searchroutes').keyup(function(){
		var current_query = $('#searchroutes').val().toLowerCase();
		if (current_query !== "") {
			$(".routes-group-list .list-group-item").hide();
			$(".routes-group-list .list-group-item").each(function(){
				var current_keyword = $(this).clone().children().remove().end().text().toLowerCase();;
				if (current_keyword.indexOf(current_query) >=0) {
					$(this).show();    	 	
				};
			});   	
		} else {
			$(".routes-group-list .list-group-item").show();
		};
	});
	
	$('body').on('click', '.routes-group-list a', function() {
		var stationName = $(this).clone().children().remove().end().text();
		
		routeModal_title.html("<span class=\"glyphicon glyphicon-info-sign\"></span>Routes departuring from '" + stationName + "'");
		
		var $routesavailable = $routes_group[stationName];
		
		routeModal_body.html("<div class=\"list-group routes-list\"></div>");
		
		$(".routes-list").empty();
		$.each($routesavailable, function(key, value) {
			$(".routes-list").append(
				"<a href=\"javascript:void(0)\" class=\"list-group-item justify-content-between\" data-route-id=\"" + $(value).find("RouteId").text() + "\">"
				+ $(value).find("RouteName").text()
				+ "<span class=\"routes-number-badge form-control-feedback\"><span class=\"badge badge-default badge-pill\">" + $(value).find("RouteNumber").text() + "</span></span>"
				+ "</a>"
			);
		});
		$('#routeModal').modal("show");
	});
	
	$('body').on('click', '#routeModal .routes-list a', function() {
		var route_id = $(this).data("route-id");
		jQuery.ajax({
			type: "GET",
			url: domain + "/api/Route/GetRouteDirections?routeId=" + route_id,
			headers: {          
				Accept: "application/xml"  
			},
			dataType: "text",
            beforeSend: function () {
               	$('#loading').modal("show");
            },
			success: function (directionsInformation) {
				jQuery.ajax({
					type: "GET",
					url: domain + "/api/Route/GetRoute?routeId=" + route_id,
					headers: {          
						Accept: "application/xml"  
					},
					dataType: "text",
					success: function (routeInformation) {
						
						var routeDirections = $.parseXML(directionsInformation),
							$routeDirections = $(routeDirections);
						var $directions = $routeDirections.find('DirectionModel');
						
						var routeInfo = $.parseXML(routeInformation),
							$routeInfo = $(routeInfo);
						var $route = $routeInfo.find('RouteModel');
						
		               	$('#loading').modal("hide");
						routeInfoModal_title.html("<span class=\"glyphicon glyphicon-info-sign\"></span>" + $route.find("RouteName").text());
						
						$("#routeInfoModal .modal-body #route_id").html($route.find("RouteId").text());
						$("#routeInfoModal .modal-body #route_number").html($route.find("RouteNumber").text());
						$("#routeInfoModal .modal-body #route_type").html($route.find("RouteType").text());
						$("#routeInfoModal .modal-body #direction_from").html($directions.eq(0).find('DirectionName').text()).data("data-direction-id", $directions.eq(0).find('DirectionId').text());
						$("#routeInfoModal .modal-body #direction_towards").html($directions.eq(1).find('DirectionName').text()).data("data-direction-id", $directions.eq(1).find('DirectionId').text());
						
						$('#routeInfoModal').modal("show");
					}
				});
			}
		});
	});
	
	$('body').on('click', '#routeInfoModal #swap-directions', function() {
		var direction_from = $("#routeInfoModal .modal-body #direction_from").html();
		var direction_from_id = $("#routeInfoModal .modal-body #direction_from").data("data-direction-id");
		var direction_towards = $("#routeInfoModal .modal-body #direction_towards").html();
		var direction_towards_id = $("#routeInfoModal .modal-body #direction_towards").data("data-direction-id");
		
		$("#routeInfoModal .modal-body #direction_from").html(direction_towards).data("data-direction-id", direction_towards_id);
		$("#routeInfoModal .modal-body #direction_towards").html(direction_from).data("data-direction-id", direction_from_id);
	});
	
	
	$('body').on('click', '#routeInfoModal #get-directions', function() {
		var route_id = $("#routeInfoModal .modal-body #route_id").html();
		var direction_id = $("#routeInfoModal .modal-body #direction_towards").data("data-direction-id");
		jQuery.ajax({
			type: "GET",
			url: domain + "/api/Route/GetRouteNextRun?routeId=" + route_id + "&directionId=" + direction_id,
			headers: {          
				Accept: "application/xml"  
			},
			dataType: "text",
            beforeSend: function () {
               	$('#routeInfoModal').modal("hide");
               	$('#routeModal').modal("hide");
               	$('#loading').modal("show");
            },
			success: function (data) {
               	$('#loading').modal("hide");
				
				var xmlDoc = $.parseXML(data),
					$xml = $(xmlDoc);
				var $scheduled_departures = $xml.find('ScheduledDeparture');
				var $estimated_departures = $xml.find('EstimatedDeparture');
				var $stops = $xml.find('Stop');
				
				var stops_json = "{\n";
				stops_json +="  \"stops\": [\n";
				for(var i = 0; i < $stops.length - 2; i++) {
                    var scheduled_departure = $scheduled_departures.eq(i).text().toString().split("T");
                    var scheduled_departure_date = scheduled_departure[0];
                    var scheduled_departure_time = (scheduled_departure[1].split("+"))[0];
                    
                    var estimated_departure = $estimated_departures.eq(i).text().toString().split("T");
                    var estimated_departure_date = estimated_departure[0];
                    var estimated_departure_time = (estimated_departure[1].split("+"))[0];
					stops_json += "    { \n";
					stops_json += "        \"stop_name\": \"" + $stops.eq(i).find("StopName").text() + "\",\n";
					stops_json += "        \"stop_id\": " + $stops.eq(i).find("StopId").text() + ",\n";
					stops_json += "        \"stop_latitude\": " + $stops.eq(i).find("StopLatitude").text() + ",\n";
					stops_json += "        \"stop_longitude\": " + $stops.eq(i).find("StopLongitude").text() + ",\n";
					stops_json += "        \"scheduled_departure\": \"" + scheduled_departure_date + ", " + scheduled_departure_time + "\",\n";
					stops_json += "        \"estimated_departure\": \"" + estimated_departure_date + ", " + estimated_departure_time + "\"\n";
					stops_json += "    },\n";
				}
                var scheduled_departure = $scheduled_departures.eq($stops.length - 1).text().toString().split("T");
                var scheduled_departure_date = scheduled_departure[0];
                var scheduled_departure_time = (scheduled_departure[1].split("+"))[0];
                
                var estimated_departure = $estimated_departures.eq($stops.length - 1).text().toString().split("T");
                var estimated_departure_date = estimated_departure[0];
                var estimated_departure_time = (estimated_departure[1].split("+"))[0];
				stops_json += "    { \n";
				stops_json += "        \"stop_name\": \"" + $stops.eq($stops.length - 1).find("StopName").text() + "\",\n";
				stops_json += "        \"stop_id\": " + $stops.eq($stops.length - 1).find("StopId").text() + ",\n";
				stops_json += "        \"stop_latitude\": " + $stops.eq($stops.length - 1).find("StopLatitude").text() + ",\n";
				stops_json += "        \"stop_longitude\": " + $stops.eq($stops.length - 1).find("StopLongitude").text() + ",\n";
                stops_json += "        \"scheduled_departure\": \"" + scheduled_departure_date + ", " + scheduled_departure_time + "\",\n";
                stops_json += "        \"estimated_departure\": \"" + estimated_departure_date + ", " + estimated_departure_time + "\"\n";
				stops_json += "    }\n";
				stops_json += "  ]\n";
				stops_json += "}\n";
                
				startSimulation(stops_json);
			}
		});
	});
});
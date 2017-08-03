using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusTrackerWeb.Models
{
    public class SearchRouteModel
    {
        public RouteModel Route { get; set; }

        public List<DirectionModel> Directions { get; set; }

        public SearchRouteModel ( RouteModel route, List<DirectionModel> directions )
        {
            Route = route;
            Directions = directions;
        } 
    }
}